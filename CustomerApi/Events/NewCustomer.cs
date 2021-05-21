using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Events
{
    public record NewCustomer(string Name, string Email, string Phone, string BillingAddress, string ShippingAddress,
        decimal CreditStanding, DateTime TimeStamp) : IEvent
    {
        public static string EventTypeStatic => "NewCustomer";
        public string EventType => EventTypeStatic;
    }
}