using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.Domain.Repository.Facade;

namespace OnlineRetailer.Domain.Repository
{
    public class EventStoreRepository : IEventRepository
    {
        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<EventStoreRepository> _logger;

        public EventStoreRepository(ILogger<EventStoreRepository> logger, EventClient eventStoreClient)
        {
            _logger = logger;
            _eventStoreClient = eventStoreClient.Client;
        }

        public async Task<bool> ExistsAsync(string streamId)
        {
            var result = _eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                streamId,
                10,
                20);

            return await result.ReadState != ReadState.StreamNotFound;
        }

        public async Task<List<ResolvedEvent>> GetAllByStreamIdAsync(string streamId)
        {
            var exists = await ExistsAsync(streamId);
            if (!exists) throw new EventStreamNotFound(streamId);

            var result = _eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                streamId,
                StreamPosition.Start);

            var events = await result.ToListAsync();
            return events;
        }

        public async Task<List<ResolvedEvent>> GetAllAsync()
        {
            var result = _eventStoreClient.ReadAllAsync(
                Direction.Forwards,
                Position.Start);

            var events = await result.ToListAsync();
            return events;
        }

        public async Task ApplyAsync<T>(T evnt, string streamId) where T : IEvent
        {
            var eventData = new EventData(
                Uuid.NewUuid(),
                evnt.EventType,
                JsonSerializer.SerializeToUtf8Bytes(evnt)
            );
            _logger.Log(LogLevel.Debug, $"{evnt.EventType} Event Was Created");

            await _eventStoreClient.AppendToStreamAsync(
                streamId,
                StreamState.Any,
                new[] {eventData}
            );
            _logger.Log(LogLevel.Debug, $"{evnt.EventType} Event Was Appended to stream: {streamId}");
        }
    }
}