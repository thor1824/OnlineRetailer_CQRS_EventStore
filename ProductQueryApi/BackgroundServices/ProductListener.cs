using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Aggregates;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.ProductQueryApi.Query.Facades;

namespace OnlineRetailer.ProductQueryApi.BackgroundServices
{
    public class ProductListener : BackgroundService
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<ProductListener> _logger;

        private readonly IServiceProvider _provider;

        public ProductListener(IServiceProvider provider, EventClient eventClient, ILogger<ProductListener> logger)
        {
            _provider = provider;
            _logger = logger;
            _eventStoreClient = eventClient.Client;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // notify
            Task.Run(() =>
            {
                var stockValidationFilter = new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix(WaitingForIsInStockValidation.EVENT_TYPE));

                _eventStoreClient.SubscribeToAllAsync(
                    Position.End,
                    async (subscription, evnt, cancellationToken) =>
                    {
                        _logger.Log(LogLevel.Debug,
                            $"DReceived event {evnt.Event.ContentType}@{evnt.OriginalStreamId}");
                        var stockValidation =
                            JsonSerializer.Deserialize<WaitingForIsInStockValidation>(evnt.Event.Data.ToArray());
                        await HandleStockValidation(evnt.OriginalStreamId, stockValidation, cancellationToken);
                    },
                    filterOptions: stockValidationFilter,
                    cancellationToken: stoppingToken
                );

                lock (this)
                {
                    Monitor.Wait(this);
                }
            });


            return Task.CompletedTask;
        }

        private async Task HandleStockValidation(string streamId, WaitingForIsInStockValidation msg,
            CancellationToken stoppingToken)
        {
            Console.WriteLine($"Validating stock for Order {streamId}");
            using (var scope = _provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var query = services.GetService<IProductQuery>();
                var eventRepo = services.GetService<IEventRepository>();


                foreach (var line in msg.OrderLines)
                {
                    var tempVerdict = await ProductInStockValidation(line, query);
                    if (tempVerdict == StockValidationStatus.ProductDoesNotExists)
                    {
                        // NotExist and escape
                        var notExistEvnt = new DoesNotExist(DateTime.UtcNow);
                        await eventRepo.ApplyAsync(notExistEvnt, streamId);
                        return;
                    }

                    if (tempVerdict == StockValidationStatus.NotInStuck)
                    {
                        // NotInStuck and escape
                        var notInStockEvnt = new NotInStock(DateTime.UtcNow);
                        await eventRepo.ApplyAsync(notInStockEvnt, streamId);
                        return;
                    }
                }

                var evnt = new AllItemsIsInStock(DateTime.UtcNow);
                await eventRepo.ApplyAsync(evnt, streamId);
            }
        }

        private async Task<StockValidationStatus> ProductInStockValidation(OrderLine line, IProductQuery query)
        {
            Product product;


            try
            {
                var projector = await query.ByIdAsync(line.ProductId);
                product = projector.Aggregate;
            }
            catch (EventStreamNotFound)
            {
                return StockValidationStatus.ProductDoesNotExists;
            }

            if (product.ItemsInStock < line.Quantity)
            {
                _logger.Log(LogLevel.Debug, $"Product {line.ProductId} quantity too low");
                return StockValidationStatus.NotInStuck;
            }

            return StockValidationStatus.AllItemsIsInStock;
        }
    }
}