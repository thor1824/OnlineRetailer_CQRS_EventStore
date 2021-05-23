using OnlineRetailer.CustomerApi.Aggregates;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.CustomerEvents;

namespace OnlineRetailer.CustomerApi.Projections
{
    public class CustomerProjection : IProjection<Customer>
    {
        public CustomerProjection(BaseStream stream)
        {
            Stream = stream;
            Aggregate.CustomerId = stream.AggregateId;
        }

        public BaseStream Stream { get; }
        public Customer Aggregate { get; } = new();

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
            Aggregate.ShippingAddress = changeShippingAddress.NewShippingAddress;
        }

        private void Apply(RemoveCustomer _)
        {
            Aggregate.IsDeleted = true;
        }

        private void Apply(ChangePhone changePhone)
        {
            Aggregate.Phone = changePhone.NewPhoneNumber;
        }

        private void Apply(ChangeName changeName)
        {
            Aggregate.Name = changeName.NewName;
        }

        private void Apply(ChangeEmail changeEmail)
        {
            Aggregate.Email = changeEmail.NewEmail;
        }

        private void Apply(ChangeCreditStanding changeCreditStanding)
        {
            Aggregate.CreditStanding = changeCreditStanding.NewCreditStanding;
        }

        private void Apply(ChangeBillingAddress changeBillingAddress)
        {
            Aggregate.BillingAddress = changeBillingAddress.NewBillingAddress;
        }

        private void Apply(NewCustomer newCustomer)
        {
            Aggregate.Name = newCustomer.Name;
            Aggregate.Email = newCustomer.Email;
            Aggregate.Phone = newCustomer.Phone;
            Aggregate.BillingAddress = newCustomer.BillingAddress;
            Aggregate.ShippingAddress = newCustomer.ShippingAddress;
            Aggregate.CreditStanding = decimal.Zero;
        }
    }
}