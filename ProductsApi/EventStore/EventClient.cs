using EventStore.Client;

namespace OnlineRetailer.ProductsApi.EventStore
{
    public class EventClient
    {
        public EventClient(string eventStoreConnectionString)
        {
            var settings = EventStoreClientSettings
                .Create(eventStoreConnectionString);
            Client = new EventStoreClient(settings);
        }

        public EventStoreClient Client { get; }
    }
}