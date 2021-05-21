using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record ChangePhone(string NewPhoneNumber, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangePhoneNumber";
        public string EventType => EventTypeStatic;
    }
}