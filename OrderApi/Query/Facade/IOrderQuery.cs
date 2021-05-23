using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.OrderApi.Projections;

namespace OnlineRetailer.OrderApi.Query.Facade
{
    public interface IOrderQuery
    {
        Task<OrderProjection> ByIdAsync(Guid orderId);
        Task<IEnumerable<OrderProjection>> AllAsync();
        Task<IEnumerable<OrderProjection>> ByCustomerAsync(Guid customerId);
    }
}