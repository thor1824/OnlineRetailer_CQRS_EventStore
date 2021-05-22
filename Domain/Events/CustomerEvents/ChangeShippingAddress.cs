using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record ChangeShippingAddress(string NewShippingAddress, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ShippingAddress";
        public string EventType => EVENT_TYPE;
    }
}