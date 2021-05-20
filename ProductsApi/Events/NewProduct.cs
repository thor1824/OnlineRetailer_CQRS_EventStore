using System;
using OnlineRetailer.ProductsApi.Events.Facade;

namespace OnlineRetailer.ProductsApi.Events
{
    public record NewProduct(string Name, string Category, decimal Price,
        int ItemsInStock, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "NewProduct";
        public string EventType => EventTypeStatic;
    }
}