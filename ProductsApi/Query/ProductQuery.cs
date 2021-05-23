using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Events;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.Domain.Streams;
using OnlineRetailer.ProductsApi.Projections;
using OnlineRetailer.ProductsApi.Query.Facades;

namespace OnlineRetailer.ProductsApi.Query
{
    public class ProductQuery : IProductQuery
    {
        private static readonly string _streamName = "product-";
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<ProductQuery> _logger;

        public ProductQuery(ILogger<ProductQuery> logger, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<ProductProjection> ByIdAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Querying product by Id: {id.ToString()}");

            var stream = new ProductStream(id);
            var projector = new ProductProjection(stream);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Fetching Events from stream");
            var events = await _eventRepository.GetAllByStreamIdAsync(stream.StreamId);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Applying events and projection current state");
            foreach (var evnt in events)
            {
                var e = EventDeserializer.DeserializeProductEvent(evnt.Event);
                projector.ApplyEvent(e);
            }

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: was projected successfully");
            return projector;
        }

        public async Task<IEnumerable<ProductProjection>> AllAsync()
        {
            _logger.Log(LogLevel.Debug, "Querying all Products");
            var dic = new Dictionary<string, ProductProjection>();

            _logger.Log(LogLevel.Debug, "Fetching all Events");
            var events = await _eventRepository.GetAllAsync();

            _logger.Log(LogLevel.Debug, "Projection current state of products");
            foreach (var evnt in events) HandleEvent(evnt, dic);

            return dic.Values.ToList();
        }

        private void HandleEvent(ResolvedEvent evnt, Dictionary<string, ProductProjection> dic)
        {
            _logger.Log(LogLevel.Debug, $"Handling event from stream: {evnt.Event.EventStreamId}");
            if (evnt.Event.EventStreamId.StartsWith(_streamName)) HandleProductEvent(evnt, dic);
        }

        private void HandleProductEvent(ResolvedEvent evnt, Dictionary<string, ProductProjection> dic)
        {
            if (!dic.ContainsKey(evnt.Event.EventStreamId))
            {
                var id = Guid.Parse(evnt.Event.EventStreamId.Replace(_streamName, ""));
                var newStream = new ProductStream(id);
                var newProjector = new ProductProjection(newStream);

                dic.Add(evnt.Event.EventStreamId, newProjector);
            }


            var stream = dic[evnt.Event.EventStreamId];

            var e = EventDeserializer.DeserializeProductEvent(evnt.Event);
            stream.ApplyEvent(e);
        }
    }
}