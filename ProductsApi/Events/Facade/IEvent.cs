using System;

namespace OnlineRetailer.ProductsApi.Events.Facade
{
    public interface IEvent
    {
        public string EventType { get; }
        public DateTime TimeStamp { get; }
    }
}