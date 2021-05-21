using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.ProductsApi.Events
{
    public record Reserve(int AmountToReserved, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "Restock";
        public string EventType => EventTypeStatic;
    }
}