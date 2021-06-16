using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.Domain.Projections;

namespace OnlineRetailer.CustomerQueryApi.Query.Facade
{
    public interface ICustomerQuery
    {
        Task<CustomerProjection> ByIdAsync(Guid id);
        Task<IEnumerable<CustomerProjection>> AllAsync();
    }
}