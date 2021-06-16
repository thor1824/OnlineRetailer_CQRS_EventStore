using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnlineRetailer.CustomerApi.Command.Facade;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.CustomerEvents;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;

namespace OnlineRetailer.CustomerApi.Command
{
    public class CustomerCommand : ICustomerCommand
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<CustomerCommand> _logger;

        public CustomerCommand(ILogger<CustomerCommand> logger, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<(bool wasSucces, string message)> RemoveAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Removing Customer: {id}");
            var evnt = new RemoveCustomer(DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message, Guid guid)> AddAsync(string name, string email, string phone,
            string billingAddress, string shippingAddress)
        {
            var newId = Guid.NewGuid();
            _logger.Log(LogLevel.Debug, $"Customer was assigned: id: {newId.ToString()}," +
                                        $"Name: {name}, Email: {email}, Phone: {phone}, billingAddress: {billingAddress}, shippingAddress: {shippingAddress}");

            var evnt = new NewCustomer(name, email, phone, billingAddress, shippingAddress, DateTime.UtcNow);

            var (wasSuccess, message) = await ApplyNewAsync(newId, evnt);
            return (wasSuccess, message, newId);
        }

        public async Task<(bool wasSucces, string message)> ChangeShippingAddress(Guid id, string newShippingAddress)
        {
            _logger.Log(LogLevel.Debug, $"Alter Customer: {id}, Change Shipping Address to {newShippingAddress}");
            var evnt = new ChangeShippingAddress(newShippingAddress, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message)> ChangeBillingAddress(Guid id, string newBillingAddress)
        {
            _logger.Log(LogLevel.Debug, $"Alter Customer: {id}, Change Billing Address to {newBillingAddress}");
            var evnt = new ChangeBillingAddress(newBillingAddress, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message)> ChangePhone(Guid id, string newPhone)
        {
            _logger.Log(LogLevel.Debug, $"Alter Customer: {id}, Change phone number to {newPhone}");
            var evnt = new ChangePhoneNumber(newPhone, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message)> ChangeEmail(Guid id, string newEmail)
        {
            _logger.Log(LogLevel.Debug, $"Alter Customer: {id}, Change email to {newEmail}");
            var evnt = new ChangeEmail(newEmail, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message)> ChangeName(Guid id, string newName)
        {
            _logger.Log(LogLevel.Debug, $"Alter Customer: {id}, Change name to {newName}");
            var evnt = new ChangeName(newName, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }


        private async Task<(bool wasSucces, string message)> ApplyChangeAsync<TEvent>(Guid id, TEvent evnt)
            where TEvent : IEvent
        {
            var customerStream = new CustomerStream(id);
            var exists = await _eventRepository.ExistsAsync(customerStream.StreamId);
            if (!exists) throw new EventStreamNotFound(customerStream.StreamId);

            await _eventRepository.ApplyAsync(evnt, customerStream.StreamId);

            return (true, "Success");
        }

        private async Task<(bool wasSucces, string message)> ApplyNewAsync<TEvent>(Guid id, TEvent evnt)
            where TEvent : IEvent
        {
            var customerStream = new CustomerStream(id);

            if (await _eventRepository.ExistsAsync(customerStream.StreamId))
                throw new EventStreamAlreadyExists(customerStream.StreamId);

            await _eventRepository.ApplyAsync(evnt, customerStream.StreamId);

            return (true, "Success");
        }
    }
}