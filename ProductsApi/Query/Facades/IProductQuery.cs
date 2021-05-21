using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.ProductsApi.Projectors;

namespace OnlineRetailer.ProductsApi.Query.Facades
{
    public interface IProductQuery
    {
        Task<StandardProductProjector> ByIdAsync(Guid id);
        Task<IEnumerable<StandardProductProjector>> AllAsync();
    }
}