using System;
using OnlineRetailer.ProductsApi.Events.Facade;

namespace OnlineRetailer.ProductsApi.Events
{
    public record Restock(int AmountRestock, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "Restock";
        public string EventType => EventTypeStatic;
    }
}