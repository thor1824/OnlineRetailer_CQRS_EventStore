using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record ChangeShippingAddress(string NewShippingAddress, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ShippingAddress";
        public string EventType => EventTypeStatic;
    }
}