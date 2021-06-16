using System;

namespace OnlineRetailer.Domain.Aggregates
{
    public class Product
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