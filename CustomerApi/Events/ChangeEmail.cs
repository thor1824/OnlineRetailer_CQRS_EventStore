using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record ChangeEmail(string NewEmail, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangeEmail";
        public string EventType => EventTypeStatic;
    }
}