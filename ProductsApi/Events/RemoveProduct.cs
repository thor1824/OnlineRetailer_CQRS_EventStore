using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.ProductsApi.Events
{
    public record RemoveProduct(DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "RemoveProduct";
        public string EventType => EventTypeStatic;
    }
}