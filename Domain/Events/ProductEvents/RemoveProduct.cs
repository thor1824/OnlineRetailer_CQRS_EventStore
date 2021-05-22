using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.ProductEvents
{
    public record RemoveProduct(DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "RemoveProduct";
        public string EventType => EVENT_TYPE;
    }
}