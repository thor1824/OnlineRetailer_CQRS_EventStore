using System;
using OnlineRetailer.ProductsApi.Events.Facade;

namespace OnlineRetailer.ProductsApi.Events
{
    public record ChangeName(string NewName, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangeName";
        public string EventType => EventTypeStatic;
    }
}