using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.Events.ProductEvents;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;
using OnlineRetailer.OrderApi.Command.Facade;
using OnlineRetailer.OrderCommandApi.Query.Facade;

namespace OnlineRetailer.OrderCommandApi.BackgroundServices
{
    public class OrderListener : BackgroundService
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<OrderListener> _logger;

        private readonly IServiceProvider _provider;

        public OrderListener(IServiceProvider provider, EventClient eventClient, ILogger<OrderListener> logger)
        {
            _provider = provider;
            _logger = logger;
            _eventStoreClient = eventClient.Client;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                var orderSubmittedFilter = new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix(SubmitOrder.EVENT_TYPE));

                _eventStoreClient.SubscribeToAllAsync(
                    Position.End,
                    async (subscription, evnt, cancellationToken) =>
                    {
                        _logger.Log(LogLevel.Debug,
                            $"Received event {evnt.Event.EventType}@{evnt.OriginalStreamId}");
                        await RequestCustomerValidation(evnt.OriginalStreamId);
                    },
                    filterOptions: orderSubmittedFilter,
                    cancellationToken: stoppingToken);

                var validCustomerFilter = new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix("Customer_"));

                _eventStoreClient.SubscribeToAllAsync(
                    Position.End,
                    async (subscription, evnt, cancellationToken) =>
                    {
                        _logger.Log(LogLevel.Debug,
                            $"Received event {evnt.Event.EventType}@{evnt.OriginalStreamId}");
                        await HandleCustomerEvent(evnt.OriginalStreamId, evnt.Event);
                    },
                    filterOptions: validCustomerFilter,
                    cancellationToken: stoppingToken);

                var InStockFilter = new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix("InStock_"));

                _eventStoreClient.SubscribeToAllAsync(
                    Position.End,
                    async (subscription, evnt, cancellationToken) =>
                    {
                        _logger.Log(LogLevel.Debug,
                            $"Received event {evnt.Event.EventType}@{evnt.OriginalStreamId}");
                        await HandleInStockEvent(evnt.OriginalStreamId, evnt.Event);
                    },
                    filterOptions: InStockFilter,
                    cancellationToken: stoppingToken);

                var orderConfirmedFilter = new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix(OrderConfirmed.EVENT_TYPE));

                _eventStoreClient.SubscribeToAllAsync(
                    Position.End,
                    async (subscription, evnt, cancellationToken) =>
                    {
                        _logger.Log(LogLevel.Debug,
                            $"Received event {evnt.Event.EventType}@{evnt.OriginalStreamId}");
                        await HandleOrderConfirmed(evnt.OriginalStreamId, evnt.Event);
                    },
                    filterOptions: orderConfirmedFilter,
                    cancellationToken: stoppingToken);

                lock (this)
                {
                    Monitor.Wait(this);
                }
            });


            return Task.CompletedTask;
        }

        private async Task HandleOrderConfirmed(string orderStreamId, EventRecord arg2Event)
        {
            using (var scope = _provider.CreateScope())
            {
                var orderQuery = scope.ServiceProvider.GetService<IOrderQuery>();
                var eventRepo = scope.ServiceProvider.GetService<IEventRepository>();
                var orderId = Guid.Parse(orderStreamId.Replace(OrderStream.STREAM_PREFIX, ""));

                var projector = await orderQuery.ByIdAsync(orderId);
                var order = projector.Aggregate;

                foreach (var line in order.OrderLines)
                {
                    var productId = line.ProductId;
                    var productStream = new ProductStream(productId);

                    var evnt = new Reserve(line.Quantity, DateTime.UtcNow);
                    await eventRepo.ApplyAsync(evnt, productStream.StreamId);
                }
            }
        }

        private async Task RequestStockValidation(string orderStreamId)
        {
            using (var scope = _provider.CreateScope())
            {
                var orderQuery = scope.ServiceProvider.GetService<IOrderQuery>();
                var id = Guid.Parse(orderStreamId.Replace(OrderStream.STREAM_PREFIX, ""));
                var projector = await orderQuery.ByIdAsync(id);

                var order = projector.Aggregate;
                var orderCommand = scope.ServiceProvider.GetService<IOrderCommand>();
                await orderCommand.WaitingForStockValidation(id, order.OrderLines);
            }
        }

        private async Task RequestCustomerValidation(string orderStreamId)
        {
            using (var scope = _provider.CreateScope())
            {
                var orderQuery = scope.ServiceProvider.GetService<IOrderQuery>();
                var id = Guid.Parse(orderStreamId.Replace(OrderStream.STREAM_PREFIX, ""));
                var projector = await orderQuery.ByIdAsync(id);

                var order = projector.Aggregate;
                var orderCommand = scope.ServiceProvider.GetService<IOrderCommand>();
                await orderCommand.WaitingForCustomerValidation(id, order.CustomerId);
            }
        }

        private async Task HandleInStockEvent(string orderStreamId, EventRecord eventRecord)
        {
            if (eventRecord.EventType == WaitingForIsInStockValidation.EVENT_TYPE) return;

            if (eventRecord.EventType == AllItemsIsInStock.EVENT_TYPE)
            {
                await ConfirmOrderAsync(orderStreamId);
                return;
            }

            await RejectOrderAsync(orderStreamId, eventRecord.EventType);
        }

        private async Task HandleCustomerEvent(string orderStreamId, EventRecord eventRecord)
        {
            if (eventRecord.EventType == WaitingForCustomerValidation.EVENT_TYPE) return;

            if (eventRecord.EventType == CustomerValid.EVENT_TYPE)
            {
                await RequestStockValidation(orderStreamId);
                return;
            }

            await RejectOrderAsync(orderStreamId, eventRecord.EventType);
        }

        private async Task ConfirmOrderAsync(string orderStreamId)
        {
            using (var scope = _provider.CreateScope())
            {
                var orderQuery = scope.ServiceProvider.GetService<IOrderQuery>();
                var id = Guid.Parse(orderStreamId.Replace(OrderStream.STREAM_PREFIX, ""));
                var projector = await orderQuery.ByIdAsync(id);
                var order = projector.Aggregate;
                if (order.OrderStatus == OrderStatus.Cancelled ||
                    order.OrderStatus == OrderStatus.Confirmed ||
                    order.OrderStatus == OrderStatus.Rejected)
                    return;

                if (order.CustomerValidationStatus == CustomerValidationStatus.CustomerValid &&
                    order.StockValidationStatus == StockValidationStatus.AllItemsIsInStock)
                {
                    var orderCommand = scope.ServiceProvider.GetService<IOrderCommand>();
                    await orderCommand.ConfirmAsync(id);
                }
            }
        }

        private async Task RejectOrderAsync(string orderStreamId, string reason)
        {
            using (var scope = _provider.CreateScope())
            {
                var orderCommand = scope.ServiceProvider.GetService<IOrderCommand>();
                var id = Guid.Parse(orderStreamId.Replace(OrderStream.STREAM_PREFIX, ""));
                await orderCommand.RejectAsync(id, reason);
            }
        }
    }
}