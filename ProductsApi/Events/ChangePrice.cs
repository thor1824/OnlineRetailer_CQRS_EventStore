using System;
using OnlineRetailer.ProductsApi.Events.Facade;

namespace OnlineRetailer.ProductsApi.Events
{
    public record ChangePrice(decimal NewPrice, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangePrice";
        public string EventType => EventTypeStatic;
    }
}