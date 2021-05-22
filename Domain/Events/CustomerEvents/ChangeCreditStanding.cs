using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record ChangeCreditStanding(decimal NewCreditStanding, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangeCreditStanding";
        public string EventType => EVENT_TYPE;
    }
}