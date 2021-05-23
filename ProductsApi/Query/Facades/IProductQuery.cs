using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.ProductsApi.Projections;

namespace OnlineRetailer.ProductsApi.Query.Facades
{
    public interface IProductQuery
    {
        Task<ProductProjection> ByIdAsync(Guid id);
        Task<IEnumerable<ProductProjection>> AllAsync();
    }
}