using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.ProductEvents
{
    public record Restock(int AmountRestock, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "Restock";
        public string EventType => EVENT_TYPE;
    }
}