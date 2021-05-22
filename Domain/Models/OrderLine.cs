using System;

namespace OnlineRetailer.Domain.Models
{
    public class OrderLine
    {
        public int Quantity { get; set; }

        public Guid ProductId { get; set; }
    }
}