using OnlineRetailer.CustomerApi.Events;
using OnlineRetailer.CustomerApi.Projections;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.CustomerApi.Projectors
{
    public class CustomerProjector : IProjector<CustomerProjection>
    {
        public CustomerProjector(BaseStream stream)
        {
            Stream = stream;
            Projection.CustomerId = stream.Id;
        }

        public BaseStream Stream { get; }
        public CustomerProjection Projection { get; } = new();

        public void ApplyEvent(IEvent evnt)
        {
            Stream.AddEvent(evnt);
            switch (evnt)
            {
                case NewCustomer newCustomer:
                    Apply(newCustomer);
                    break;
            }
        }

        private void Apply(NewCustomer newCustomer)
        {
            Projection.Name = newCustomer.Name;
            Projection.Email = newCustomer.Email;
            Projection.Phone = newCustomer.Phone;
            Projection.BillingAddress = newCustomer.BillingAddress;
            Projection.ShippingAddress = newCustomer.ShippingAddress;
            Projection.CreditStanding = decimal.Zero;
        }
    }
}