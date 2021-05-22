using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.OrderApi.Projectors;

namespace OnlineRetailer.OrderApi.Query.Facade
{
    public interface IOrderQuery
    {
        Task<OrderProjector> ByIdAsync(Guid orderId);
        Task<IEnumerable<OrderProjector>> AllAsync();
        Task<IEnumerable<OrderProjector>> ByCustomerAsync(Guid customerId);
    }
}