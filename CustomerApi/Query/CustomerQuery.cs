using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using OnlineRetailer.CustomerApi.Projectors;
using OnlineRetailer.CustomerApi.Query.Facade;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events;
using OnlineRetailer.Domain.Events.CustomerEvents;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;

namespace OnlineRetailer.CustomerApi.Query
{
    public class CustomerQuery : ICustomerQuery
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<CustomerQuery> _logger;

        public CustomerQuery(ILogger<CustomerQuery> logger, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<CustomerProjector> ByIdAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Querying Customer by Id: {id.ToString()}");

            var stream = new CustomerStream(id);
            var projector = new CustomerProjector(stream);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Fetching Events from stream");
            var events = await _eventRepository.GetAllByStreamIdAsync(stream.StreamId);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Applying events and projection current state");
            foreach (var evnt in events)
            {
                var e = EventDeserializer.DeserializeCustomerEvent(evnt.Event);
                projector.ApplyEvent(e);
            }

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: was projected successfully");
            return projector;
        }

        public async Task<IEnumerable<CustomerProjector>> AllAsync()
        {
            _logger.Log(LogLevel.Debug, "Querying all Products");
            var dic = new Dictionary<string, CustomerProjector>();

            _logger.Log(LogLevel.Debug, "Fetching all Events");
            var events = await _eventRepository.GetAllAsync();

            _logger.Log(LogLevel.Debug, "Projection current state of products");
            foreach (var evnt in events) HandleEvent(evnt, dic);

            return dic.Values.ToList();
        }

        private void HandleEvent(ResolvedEvent evnt, Dictionary<string, CustomerProjector> dic)
        {
            _logger.Log(LogLevel.Debug, $"Handling event from stream: {evnt.Event.EventStreamId}");
            if (evnt.Event.EventStreamId.StartsWith(CustomerStream.STREAM_PREFIX)) HandleCustomerEvent(evnt, dic);
        }

        private void HandleCustomerEvent(ResolvedEvent evnt, Dictionary<string, CustomerProjector> dic)
        {
            if (!dic.ContainsKey(evnt.Event.EventStreamId))
            {
                var id = Guid.Parse(evnt.Event.EventStreamId.Replace(CustomerStream.STREAM_PREFIX, ""));
                var newStream = new CustomerStream(id);
                var newProjector = new CustomerProjector(newStream);

                dic.Add(evnt.Event.EventStreamId, newProjector);
            }


            var stream = dic[evnt.Event.EventStreamId];

            var e = EventDeserializer.DeserializeCustomerEvent(evnt.Event);
            stream.ApplyEvent(e);
        }
    }
}