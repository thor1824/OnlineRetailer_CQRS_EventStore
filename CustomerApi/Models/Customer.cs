using System;
using System.Collections.Generic;

namespace OnlineRetailer.CustomerApi.Models
{
    public class CustomerProjection 
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public decimal? CreditStanding { get; set; }
    }

    public class Customer : IProjector
    {
        public DateTime LastModified { get; }
        public IList<ICustomerEvent> EventStream { get; }
        private CustomerProjection _customerProjection = new();
        public void ApplyEvent(ICustomerEvent evnt)
        {
            EventStream.Add(evnt);
            switch (evnt)
            {
                case NewCustomer newCustomer:
                    Apply(newCustomer);
                    break;
            }
        }

        private void Apply(NewCustomer newCustomer)
        {
            _customerProjection.Name = newCustomer.Name;
            _customerProjection.Email = newCustomer.Email;
            _customerProjection.Phone = newCustomer.Phone;
            _customerProjection.BillingAddress = newCustomer.BillingAddress;
            _customerProjection.ShippingAddress = newCustomer.ShippingAddress;
        }
    }

    public interface IProjector
    {
        
        DateTime LastModified { get; }
        IList<ICustomerEvent> EventStream { get; }

        void ApplyEvent(ICustomerEvent evnt);

    }

    public interface ICustomerEvent
    {
    }

    public record NewCustomer(string Name, string Email, string Phone, string BillingAddress, string ShippingAddress, decimal CreditStanding) : ICustomerEvent;
}

