using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.CustomerApi.Projectors;

namespace OnlineRetailer.CustomerApi.Query.Facade
{
    public interface ICustomerQuery
    {
        Task<CustomerProjector> ByIdAsync(Guid id);
        Task<IEnumerable<CustomerProjector>> AllAsync();
    }
}