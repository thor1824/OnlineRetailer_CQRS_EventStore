using System;
using System.Threading.Tasks;

namespace OnlineRetailer.CustomerApi.Command.Facade
{
    public interface ICustomerCommand
    {
        Task<(bool wasSucces, string message)> RemoveAsync(Guid id);

        Task<(bool wasSucces, string message, Guid guid)> AddAsync(string name, string category, decimal price,
            int itemsInStock);

        Task<(bool wasSucces, string message)> ChangeShippingAddress(Guid guid, string newShippingAddress);
        Task<(bool wasSucces, string message)> ChangeBillingAddress(Guid guid, string newBillingAddress);
        Task<(bool wasSucces, string message)> ChangePhone(Guid guid, object newPhone);
        Task<(bool wasSucces, string message)> ChangeEmail(Guid guid, string newEmail);
        Task<(bool wasSucces, string message)> ChangeName(Guid guid, string newName);
    }
}