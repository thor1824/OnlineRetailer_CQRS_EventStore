using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Events.CustomerEvents
{
    public record NewCustomer(string Name, string Email, string Phone, string BillingAddress, string ShippingAddress,
        DateTime TimeStamp) : IEvent
    {
        public static string EVENT_TYPE => "NewCustomer";
        public string EventType => EVENT_TYPE;
    }
}