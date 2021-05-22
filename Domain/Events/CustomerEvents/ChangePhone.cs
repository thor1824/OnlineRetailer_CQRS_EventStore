using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record ChangePhone(string NewPhoneNumber, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangePhoneNumber";
        public string EventType => EVENT_TYPE;
    }
}