using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using OnlineRetailer.ProductsApi.Events;
using OnlineRetailer.ProductsApi.Events.Facade;
using OnlineRetailer.ProductsApi.EventStore.Repository.Facade;
using OnlineRetailer.ProductsApi.Exceptions;
using OnlineRetailer.ProductsApi.Models;
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

        public async Task<ProductStream> ByIdAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Querying product by Id: {id.ToString()}");

            var stream = new ProductStream(id);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Fetching Events from stream");
            var events = await _eventRepository.GetAllByStreamIdAsync(stream.StreamId);

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: Applying events and projection current state");
            foreach (var evnt in events)
            {
                var e = DeserializeEvent(evnt.Event);
                stream.ApplyEvent(e);
            }

            _logger.Log(LogLevel.Debug, $"{stream.StreamId}: was projected successfully");
            return stream;
        }

        public async Task<IEnumerable<ProductStream>> AllAsync()
        {
            _logger.Log(LogLevel.Debug, "Querying all Products");
            var dic = new Dictionary<string, ProductStream>();

            _logger.Log(LogLevel.Debug, "Fetching all Events");
            var events = await _eventRepository.GetAllAsync();

            _logger.Log(LogLevel.Debug, "Projection current state of products");
            foreach (var evnt in events) HandleEvent(evnt, dic);

            return dic.Values.ToList();
        }

        private void HandleEvent(ResolvedEvent evnt, Dictionary<string, ProductStream> dic)
        {
            _logger.Log(LogLevel.Debug, $"Handling event from stream: {evnt.Event.EventStreamId}");
            if (evnt.Event.EventStreamId.StartsWith(_streamName)) HandleProductEvent(evnt, dic);
        }

        private void HandleProductEvent(ResolvedEvent evnt, Dictionary<string, ProductStream> dic)
        {
            if (!dic.ContainsKey(evnt.Event.EventStreamId))
                dic.Add(evnt.Event.EventStreamId,
                    new ProductStream(Guid.Parse(evnt.Event.EventStreamId.Replace(_streamName, ""))));

            var stream = dic[evnt.Event.EventStreamId];
            var e = DeserializeEvent(evnt.Event);
            stream.ApplyEvent(e);
        }

        private IEvent DeserializeEvent(EventRecord record)
        {
            if (record.EventType == NewProduct.EventTypeStatic)
                return JsonSerializer.Deserialize<NewProduct>(record.Data.ToArray());

            if (record.EventType == RemoveProduct.EventTypeStatic)
                return JsonSerializer.Deserialize<RemoveProduct>(record.Data.ToArray());

            if (record.EventType == ChangeName.EventTypeStatic)
                return JsonSerializer.Deserialize<ChangeName>(record.Data.ToArray());
            
            if (record.EventType == ChangeCategory.EventTypeStatic)
                return JsonSerializer.Deserialize<ChangeCategory>(record.Data.ToArray());
            
            if (record.EventType == ChangePrice.EventTypeStatic)
                return JsonSerializer.Deserialize<ChangePrice>(record.Data.ToArray());
            
            if (record.EventType == Restock.EventTypeStatic)
                return JsonSerializer.Deserialize<Restock>(record.Data.ToArray());
            
            if (record.EventType == Reserve.EventTypeStatic)
                return JsonSerializer.Deserialize<Reserve>(record.Data.ToArray());

            throw new EventTypeNotSupportedException($"Event of type {record.EventType}, not supported");
        }
    }
}