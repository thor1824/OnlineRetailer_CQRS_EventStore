using System;

namespace OnlineRetailer.Domain.Aggregates
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public decimal? CreditStanding { get; set; }
        public bool IsDeleted { get; set; }
    }
}