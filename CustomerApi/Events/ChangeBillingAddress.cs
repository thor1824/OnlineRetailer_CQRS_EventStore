using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record ChangeBillingAddress(string NewBillingAddress, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangeBillingAddress";
        public string EventType => EventTypeStatic;
    }
}