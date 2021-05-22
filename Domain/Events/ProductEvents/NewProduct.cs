using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.ProductEvents
{
    public record NewProduct(string Name, string Category, decimal Price,
        int ItemsInStock, DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "NewProduct";
        public string EventType => EVENT_TYPE;
    }
}