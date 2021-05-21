using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.ProductsApi.Events
{
    public record ChangeName(string NewName, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangeName";
        public string EventType => EventTypeStatic;
    }
}