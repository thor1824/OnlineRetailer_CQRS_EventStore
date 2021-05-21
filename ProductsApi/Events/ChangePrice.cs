using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.ProductsApi.Events
{
    public record ChangePrice(decimal NewPrice, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "ChangePrice";
        public string EventType => EventTypeStatic;
    }
}