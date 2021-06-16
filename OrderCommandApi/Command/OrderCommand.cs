using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;
using OnlineRetailer.OrderApi.Command.Facade;
using OnlineRetailer.OrderCommandApi.Query.Facade;

namespace OnlineRetailer.OrderCommandApi.Command
{
    public class OrderCommand : IOrderCommand
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<OrderCommand> _logger;
        private readonly IOrderQuery _query;

        public OrderCommand(ILogger<OrderCommand> logger, IEventRepository eventRepository, IOrderQuery query)
        {
            _logger = logger;
            _eventRepository = eventRepository;
            _query = query;
        }

        public async Task<(bool wasSuccess, string message, Guid id)> PlaceAsync(Guid customerId,
            IList<OrderLine> orderLines, CancellationToken cancellationToken)
        {
            var newId = Guid.NewGuid();
            _logger.Log(LogLevel.Debug, $"Order was assigned id: {newId.ToString()}, and marked as submitted");

            var submitEvnt = new SubmitOrder(customerId, orderLines, DateTime.UtcNow);

            await ApplyNewAsync(newId, submitEvnt);

            var (isConfirmed, statusMessage) = await _query.IsConfirmed(newId, cancellationToken);
            return !isConfirmed ? (false, statusMessage, newId) : (true, "Order was placed", newId);
        }

        public async Task<(bool wasSuccess, string message)> CancelAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Trying to cancel, Order: {id.ToString()}");

            var cancelEvnt = new OrderCancelled(DateTime.UtcNow);

            return await ApplyChangeAsync(id, cancelEvnt);
        }

        public async Task<(bool wasSucces, string message)> ConfirmAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Trying to Confirm, Order: {id.ToString()}");

            var confirmEvnt = new OrderConfirmed(DateTime.UtcNow);

            return await ApplyChangeAsync(id, confirmEvnt);
        }

        public async Task<(bool wasSucces, string message)> RejectAsync(Guid id, string reason)
        {
            _logger.Log(LogLevel.Debug, $"Trying to Reject, Order: {id.ToString()}");

            var rejectEvnt = new OrderRejected(reason, DateTime.UtcNow);

            return await ApplyChangeAsync(id, rejectEvnt);
        }

        public async Task<(bool wasSucces, string message)> WaitingForStockValidation(Guid orderId,
            IList<OrderLine> orderLines)
        {
            _logger.Log(LogLevel.Debug, $"Marking Order: {orderId.ToString()} for StockValidation");

            _logger.Log(LogLevel.Debug, $"Trying to Reject, Order: {orderId.ToString()}");

            var stockValidation = new WaitingForIsInStockValidation(orderLines, DateTime.UtcNow);

            return await ApplyChangeAsync(orderId, stockValidation);
        }

        public async Task<(bool wasSucces, string message)> WaitingForCustomerValidation(Guid orderId, Guid customerId)
        {
            _logger.Log(LogLevel.Debug, $"Marking Order: {orderId.ToString()} for CustomerValidation");

            var customerValidation = new WaitingForCustomerValidation(customerId, DateTime.UtcNow);

            return await ApplyChangeAsync(orderId, customerValidation);
        }

        

        private async Task<(bool wasSucces, string message)> ApplyChangeAsync<TEvent>(Guid id, TEvent evnt)
            where TEvent : IEvent
        {
            var customerStream = new OrderStream(id);
            var exists = await _eventRepository.ExistsAsync(customerStream.StreamId);
            if (!exists) throw new EventStreamNotFound(customerStream.StreamId);

            await _eventRepository.ApplyAsync(evnt, customerStream.StreamId);

            return (true, "Success");
        }

        private async Task<(bool wasSucces, string message)> ApplyNewAsync<TEvent>(Guid id, TEvent evnt)
            where TEvent : IEvent
        {
            var customerStream = new OrderStream(id);

            var exists = await _eventRepository.ExistsAsync(customerStream.StreamId);

            if (exists) throw new EventStreamAlreadyExists(customerStream.StreamId);

            await _eventRepository.ApplyAsync(evnt, customerStream.StreamId);

            return (true, "Success");
        }
    }
}