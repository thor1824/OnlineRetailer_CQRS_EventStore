using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OnlineRetailer.Domain.Models;

namespace OnlineRetailer.OrderApi.Command.Facade
{
    public interface IOrderCommand
    {
        Task<(bool wasSuccess, string message, Guid id)> PlaceAsync(Guid newOrderCustomerId,
            IList<OrderLine> newOrderOrderLines, CancellationToken cancellationToken);
        Task<(bool wasSuccess, string message)> CancelAsync(Guid guid);
        Task<(bool wasSucces, string message)> ConfirmAsync(Guid id);
        Task<(bool wasSucces, string message)> RejectAsync(Guid id, string reason);
        Task<(bool wasSucces, string message)> WaitingForStockValidation(Guid id, IList<OrderLine> orderLines);
        Task<(bool wasSucces, string message)> WaitingForCustomerValidation(Guid orderId, Guid customerId);
    }
}