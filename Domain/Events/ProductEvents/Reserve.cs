using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.ProductEvents
{
    public record Reserve(int AmountToReserved, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "Reserve";
        public string EventType => EVENT_TYPE;
    }
}