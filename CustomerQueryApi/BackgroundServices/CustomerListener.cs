using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineRetailer.CustomerQueryApi.Query.Facade;
using OnlineRetailer.Domain.Aggregates;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.Domain.Repository.Facade;

namespace OnlineRetailer.CustomerQueryApi.BackgroundServices
{
    public class CustomerListener : BackgroundService
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<CustomerListener> _logger;

        private readonly IServiceProvider _provider;

        public CustomerListener(IServiceProvider provider, EventClient eventClient, ILogger<CustomerListener> logger)
        {
            _provider = provider;
            _logger = logger;
            _eventStoreClient = eventClient.Client;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                var customerValidationFilter = new SubscriptionFilterOptions(
                    EventTypeFilter.Prefix(WaitingForCustomerValidation.EVENT_TYPE));

                _eventStoreClient.SubscribeToAllAsync(
                    Position.End,
                    async (subscription, evnt, cancellationToken) =>
                    {
                        _logger.Log(LogLevel.Debug,
                            $"Received event {evnt.Event.ContentType}@{evnt.OriginalStreamId}");
                        var stockValidation =
                            JsonSerializer.Deserialize<WaitingForCustomerValidation>(evnt.Event.Data.ToArray());
                        await HandleCustomerValidation(evnt.OriginalStreamId, stockValidation, cancellationToken);
                    },
                    filterOptions: customerValidationFilter,
                    cancellationToken: stoppingToken);

                lock (this)
                {
                    Monitor.Wait(this);
                }
            });

            return Task.CompletedTask;
        }

        private async Task HandleCustomerValidation(string orderStreamId, WaitingForCustomerValidation stockValidation,
            CancellationToken cancellationToken)
        {
            using (var scope = _provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var query = services.GetService<ICustomerQuery>();
                var eventRepo = services.GetService<IEventRepository>();

                var tempVerdict = await ProductInStockValidation(stockValidation.CustomerId, query);
                if (tempVerdict == CustomerValidationStatus.NotFound)
                {
                    var notFoundEvnt = new CustomerNotFound(DateTime.UtcNow);
                    await eventRepo.ApplyAsync(notFoundEvnt, orderStreamId);
                    return;
                }

                if (tempVerdict == CustomerValidationStatus.BadCreditStanding)
                {
                    var badCredit = new CustomerBadCredit(DateTime.UtcNow);
                    await eventRepo.ApplyAsync(badCredit, orderStreamId);
                    return;
                }

                var evnt = new CustomerValid(DateTime.UtcNow);
                await eventRepo.ApplyAsync(evnt, orderStreamId);
            }
        }


        private async Task<CustomerValidationStatus> ProductInStockValidation(Guid customerId, ICustomerQuery query)
        {
            Customer customer;
            try
            {
                var projector = await query.ByIdAsync(customerId);
                customer = projector.Aggregate;
            }
            catch (EventStreamNotFound)
            {
                _logger.Log(LogLevel.Debug, "Customer Not Found");

                return CustomerValidationStatus.NotFound;
            }

            if (customer.CreditStanding < 0)
            {
                _logger.Log(LogLevel.Debug, "Customer, bad credit standing");
                return CustomerValidationStatus.BadCreditStanding;
            }

            _logger.Log(LogLevel.Debug, "Customer Valid");
            return CustomerValidationStatus.CustomerValid;
        }
    }
}