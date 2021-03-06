using System;
using System.Threading.Tasks;

namespace OnlineRetailer.ProductCommandApi.Command.Facades
{
    public interface IProductCommand
    {
        Task<(bool wasSucces, string message)> RemoveAsync(Guid id);

        Task<(bool wasSucces, string message, Guid guid)> AddAsync(string name, string category, decimal price,
            int itemsInStock);

        Task<(bool wasSucces, string message)> IncreaseStockAsync(Guid id, int amountRestocked);

        Task<(bool wasSucces, string message)> ReserveStuckAsync(Guid id, int amountReserved);

        Task<(bool wasSucces, string message)> RenameAsync(Guid id, string name);
        Task<(bool wasSucces, string message)> ChangeCategoryAsync(Guid id, string category);
        Task<(bool wasSucces, string message)> ChangePriceAsync(Guid id, decimal price);
    }
}