using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record RemoveCustomer(DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "RemoveCustomer";
        public string EventType => EVENT_TYPE;
    }
}