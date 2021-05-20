using System;
using OnlineRetailer.ProductsApi.Events.Facade;

namespace OnlineRetailer.ProductsApi.Events
{
    public record RemoveProduct(DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "RemoveProduct";
        public string EventType => EventTypeStatic;
    }
}