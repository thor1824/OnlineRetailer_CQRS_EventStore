using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OnlineRetailer.Domain.Projections;

namespace OnlineRetailer.OrderCommandApi.Query.Facade
{
    public interface IOrderQuery
    {
        Task<OrderProjection> ByIdAsync(Guid orderId);
        Task<IEnumerable<OrderProjection>> AllAsync();
        Task<IEnumerable<OrderProjection>> ByCustomerAsync(Guid customerId);

        Task<(bool isConfirmed, string message)> IsConfirmed(Guid newId, CancellationToken cancellationToken);
    }
}