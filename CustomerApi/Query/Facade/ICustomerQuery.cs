using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.CustomerApi.Projections;

namespace OnlineRetailer.CustomerApi.Query.Facade
{
    public interface ICustomerQuery
    {
        Task<CustomerProjection> ByIdAsync(Guid id);
        Task<IEnumerable<CustomerProjection>> AllAsync();
    }
}