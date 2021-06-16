using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.Domain.Projections;

namespace OnlineRetailer.ProductQueryApi.Query.Facades
{
    public interface IProductQuery
    {
        Task<ProductProjection> ByIdAsync(Guid id);
        Task<IEnumerable<ProductProjection>> AllAsync();
    }
}