using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record RemoveCustomer(DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "RemoveCustomer";
        public string EventType => EventTypeStatic;
    }
}