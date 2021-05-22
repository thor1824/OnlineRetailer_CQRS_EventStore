using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record ChangeEmail(string NewEmail, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangeEmail";
        public string EventType => EVENT_TYPE;
    }
}