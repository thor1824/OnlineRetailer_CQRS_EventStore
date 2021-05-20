using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineRetailer.ProductsApi.Models;

namespace OnlineRetailer.ProductsApi.Query.Facades
{
    public interface IProductQuery
    {
        Task<ProductStream> ByIdAsync(Guid id);
        Task<IEnumerable<ProductStream>> AllAsync();
    }
}