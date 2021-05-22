using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record ChangeName(string NewName, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangeName";
        public string EventType => EVENT_TYPE;
    }
}