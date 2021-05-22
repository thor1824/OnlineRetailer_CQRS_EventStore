using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;
using OnlineRetailer.OrderApi.Projectors;
using OnlineRetailer.OrderApi.Query.Facade;

namespace OnlineRetailer.OrderApi.Query
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

        public async Task<OrderProjector> ByIdAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Querying Order by Id: {id.ToString()}");

            var stream = new OrderStream(id);
            
            var projector = new OrderProjector(stream);

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

        public async Task<IEnumerable<OrderProjector>> AllAsync()
        {
            _logger.Log(LogLevel.Debug, "Querying all Products");
            var dic = new Dictionary<string, OrderProjector>();

            _logger.Log(LogLevel.Debug, "Fetching all Events");
            var events = await _eventRepository.GetAllAsync();

            _logger.Log(LogLevel.Debug, "Projection current state of products");
            foreach (var evnt in events) HandleEvent(evnt, dic);

            return dic.Values.ToList();
        }

        public async Task<IEnumerable<OrderProjector>> ByCustomerAsync(Guid customerId)
        {
            var ordersByCustomer = new List<OrderProjector>();
            _logger.Log(LogLevel.Debug, $"Querying Orders By CustomerID: {customerId.ToString()}");

            var allOrders = await AllAsync();
            foreach (var order in allOrders)
                if (order.Projection.CustomerId.Equals(customerId))
                    ordersByCustomer.Add(order);

            return ordersByCustomer;
        }

        private void HandleEvent(ResolvedEvent evnt, Dictionary<string, OrderProjector> dic)
        {
            _logger.Log(LogLevel.Debug, $"Handling event from stream: {evnt.Event.EventStreamId}");
            if (evnt.Event.EventStreamId.StartsWith(OrderStream.STREAM_PREFIX)) HandleOrderEvent(evnt, dic);
        }

        private void HandleOrderEvent(ResolvedEvent evnt, Dictionary<string, OrderProjector> dic)
        {
            if (!dic.ContainsKey(evnt.Event.EventStreamId))
            {
                var id = Guid.Parse(evnt.Event.EventStreamId.Replace(OrderStream.STREAM_PREFIX, ""));
                var newStream = new OrderStream(id);
                var newProjector = new OrderProjector(newStream);

                dic.Add(evnt.Event.EventStreamId, newProjector);
            }


            var stream = dic[evnt.Event.EventStreamId];

            var e = EventDeserializer.DeserializeOrderEvent(evnt.Event);
            stream.ApplyEvent(e);
        }
    }
}