using System;

namespace OnlineRetailer.ProductsApi.Models.Projections
{
    public class ProductProjection
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastModified { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int ItemsInStock { get; set; }
        public int ItemsReserved { get; set; }
    }
}