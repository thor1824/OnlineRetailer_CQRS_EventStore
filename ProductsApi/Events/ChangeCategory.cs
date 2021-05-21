using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.ProductsApi.Events
{
    public record ChangeCategory(string NewCategory, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangeCategory";
        public string EventType => EventTypeStatic;
    }
}