using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record ChangeCreditStanding(decimal NewCreditStanding, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangeCreditStanding";
        public string EventType => EventTypeStatic;
    }
}