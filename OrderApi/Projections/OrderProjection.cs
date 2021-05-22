using System;
using System.Collections.Generic;
using OnlineRetailer.Domain.Models;

namespace OnlineRetailer.OrderApi.Projections
{
    public class OrderProjection
    {
        public Guid OrderId { get; set; }
        public IList<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
        public Guid CustomerId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public CustomerValidationStatus? CustomerValidationStatus { get; set; }
        public StockValidationStatus? StockValidationStatus { get; set; }
        public string RejectionReason { get; set; }
    }
}