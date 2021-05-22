using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record ChangeBillingAddress(string NewBillingAddress, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangeBillingAddress";
        public string EventType => EVENT_TYPE;
    }
}