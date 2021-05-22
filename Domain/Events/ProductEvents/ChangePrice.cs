using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.ProductEvents
{
    public record ChangePrice(decimal NewPrice, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangePrice";
        public string EventType => EVENT_TYPE;
    }
}