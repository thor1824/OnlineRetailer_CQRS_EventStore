using OnlineRetailer.CustomerApi.Projections;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.CustomerEvents;

namespace OnlineRetailer.CustomerApi.Projectors
{
    public class CustomerProjector : IProjector<CustomerProjection>
    {
        public CustomerProjector(BaseStream stream)
        {
            Stream = stream;
            Projection.CustomerId = stream.AggregateId;
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
                case ChangeShippingAddress changeShippingAddress:
                    Apply(changeShippingAddress);
                    break;
                case ChangeBillingAddress changeBillingAddress:
                    Apply(changeBillingAddress);
                    break;
                case ChangeCreditStanding creditStanding:
                    Apply(creditStanding);
                    break;
                case ChangeEmail changeEmail:
                    Apply(changeEmail);
                    break;
                case ChangeName changeName:
                    Apply(changeName);
                    break;
                case ChangePhone changePhone:
                    Apply(changePhone);
                    break;
                case RemoveCustomer removeCustomer:
                    Apply(removeCustomer);
                    break;
            }
        }

        private void Apply(ChangeShippingAddress changeShippingAddress)
        {
            Projection.ShippingAddress = changeShippingAddress.NewShippingAddress;
        }

        private void Apply(RemoveCustomer _)
        {
            Projection.IsDeleted = true;
        }

        private void Apply(ChangePhone changePhone)
        {
            Projection.Phone = changePhone.NewPhoneNumber;
        }

        private void Apply(ChangeName changeName)
        {
            Projection.Name = changeName.NewName;
        }

        private void Apply(ChangeEmail changeEmail)
        {
            Projection.Email = changeEmail.NewEmail;
        }

        private void Apply(ChangeCreditStanding changeCreditStanding)
        {
            Projection.CreditStanding = changeCreditStanding.NewCreditStanding;
        }

        private void Apply(ChangeBillingAddress changeBillingAddress)
        {
            Projection.BillingAddress = changeBillingAddress.NewBillingAddress;
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