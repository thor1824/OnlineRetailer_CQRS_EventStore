using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.ProductEvents
{
    public record ChangeCategory(string NewCategory, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "ChangeCategory";
        public string EventType => EVENT_TYPE;
    }
}