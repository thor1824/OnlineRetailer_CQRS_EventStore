using System;
using System.Threading.Tasks;
using OnlineRetailer.CustomerApi.Command.Facade;

namespace OnlineRetailer.CustomerApi.Command
{
    public class CustomerCommand : ICustomerCommand
    {
        public Task<(bool wasSucces, string message)> RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool wasSucces, string message, Guid guid)> AddAsync(string name, string category, decimal price,
            int itemsInStock)
        {
            throw new NotImplementedException();
        }

        public Task<(bool wasSucces, string message)> ChangeShippingAddress(Guid guid, string newShippingAddress)
        {
            throw new NotImplementedException();
        }

        public Task<(bool wasSucces, string message)> ChangeBillingAddress(Guid guid, string newBillingAddress)
        {
            throw new NotImplementedException();
        }

        public Task<(bool wasSucces, string message)> ChangePhone(Guid guid, object newPhone)
        {
            throw new NotImplementedException();
        }

        public Task<(bool wasSucces, string message)> ChangeEmail(Guid guid, string newEmail)
        {
            throw new NotImplementedException();
        }

        public Task<(bool wasSucces, string message)> ChangeName(Guid guid, string newName)
        {
            throw new NotImplementedException();
        }
    }
}