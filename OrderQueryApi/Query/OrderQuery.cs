using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Events;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.Domain.Projections;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;
using OnlineRetailer.OrderQueryApi.Query.Facade;

namespace OnlineRetailer.OrderQueryApi.Query
{
    public class OrderQuery : IOrderQuery
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<OrderQuery> _logger;

        public OrderQuery(ILogger<OrderQuery> logger, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<OrderProjection> ByIdAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Querying Order by Id: {id.ToString()}");

            var stream = new OrderStream(id);

            var projector = new OrderProjection(stream);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Fetching Events from stream");
            var events = await _eventRepository.GetAllByStreamIdAsync(stream.StreamId);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Applying events and projection current state");
            foreach (var evnt in events)
            {
                var e = EventDeserializer.DeserializeOrderEvent(evnt.Event);
                projector.ApplyEvent(e);
            }

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: was projected successfully");
            return projector;
        }

        public async Task<IEnumerable<OrderProjection>> AllAsync()
        {
            _logger.Log(LogLevel.Debug, "Querying all Products");
            var dic = new Dictionary<string, OrderProjection>();

            _logger.Log(LogLevel.Debug, "Fetching all Events");
            var events = await _eventRepository.GetAllAsync();

            _logger.Log(LogLevel.Debug, "Projection current state of products");
            foreach (var evnt in events) HandleEvent(evnt, dic);

            return dic.Values.ToList();
        }

        public async Task<IEnumerable<OrderProjection>> ByCustomerAsync(Guid customerId)
        {
            var ordersByCustomer = new List<OrderProjection>();
            _logger.Log(LogLevel.Debug, $"Querying Orders By CustomerID: {customerId.ToString()}");

            var allOrders = await AllAsync();
            foreach (var order in allOrders)
                if (order.Aggregate.CustomerId.Equals(customerId))
                    ordersByCustomer.Add(order);

            return ordersByCustomer;
        }

        private void HandleEvent(ResolvedEvent evnt, Dictionary<string, OrderProjection> dic)
        {
            _logger.Log(LogLevel.Debug, $"Handling event from stream: {evnt.Event.EventStreamId}");
            if (evnt.Event.EventStreamId.StartsWith(OrderStream.STREAM_PREFIX)) HandleOrderEvent(evnt, dic);
        }

        private void HandleOrderEvent(ResolvedEvent evnt, Dictionary<string, OrderProjection> dic)
        {
            if (!dic.ContainsKey(evnt.Event.EventStreamId))
            {
                var id = Guid.Parse(evnt.Event.EventStreamId.Replace(OrderStream.STREAM_PREFIX, ""));
                var newStream = new OrderStream(id);
                var newProjector = new OrderProjection(newStream);

                dic.Add(evnt.Event.EventStreamId, newProjector);
            }


            var stream = dic[evnt.Event.EventStreamId];

            var e = EventDeserializer.DeserializeOrderEvent(evnt.Event);
            stream.ApplyEvent(e);
        }
        
        private async Task<(bool isConfirmed, string message)> IsConfirmed(Guid newId,
            CancellationToken cancellationToken)
        {
            var isConfirmed = false;
            var message = "N/A";

            var isValidationDone = false;
            while (!isValidationDone)
            {
                if (cancellationToken.IsCancellationRequested) return (false, "Operation Was Canceled");

                var projector = await ByIdAsync(newId);
                var order = projector.Aggregate;
                if (order.OrderStatus != OrderStatus.Submitted)
                {
                    isValidationDone = true;
                    isConfirmed = order.OrderStatus == OrderStatus.Confirmed;

                    switch (order.OrderStatus)
                    {
                        case OrderStatus.Confirmed:
                            message = "Order was Confirmed";
                            break;
                        case OrderStatus.Rejected:
                            message = $"Order was Rejected! Reason: {order.RejectionReason}";
                            break;
                        case OrderStatus.Cancelled:
                            message = "Order was Canceled";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return (isConfirmed, message);
        }
    }
}