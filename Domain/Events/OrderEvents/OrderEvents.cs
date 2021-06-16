using System;
using System.Collections.Generic;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Models;

namespace OnlineRetailer.Domain.Events.OrderEvents
{
    public record SubmitOrder(Guid CustomerId, IList<OrderLine> OrderLines, DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Submitted";

        public string EventType => EVENT_TYPE;
    }

    public record WaitingForCustomerValidation(Guid CustomerId, DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Customer_WaitingForValidation";

        public string EventType => EVENT_TYPE;
    }

    public record CustomerValid(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Customer_Valid";

        public string EventType => EVENT_TYPE;
    }

    public record CustomerBadCredit(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Customer_BadCredit";

        public string EventType => EVENT_TYPE;
    }

    public record CustomerNotFound(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Customer_NotFound";

        public string EventType => EVENT_TYPE;
    }

    public record WaitingForIsInStockValidation(IList<OrderLine> OrderLines, DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "InStock_WaitingValidation";

        public string EventType => EVENT_TYPE;
    }

    public record AllItemsIsInStock(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "InStock_All";

        public string EventType => EVENT_TYPE;
    }

    public record NotInStock(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "InStock_None";

        public string EventType => EVENT_TYPE;
    }

    public record DoesNotExist(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "InStock_DoesNotExist";

        public string EventType => EVENT_TYPE;
    }

    public record PartialInStock(IList<OrderLine> OrderLinesNotInStock, DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "InStock_Partial";

        public string EventType => EVENT_TYPE;
    }

    public record OrderConfirmed(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Confirmed";

        public string EventType => EVENT_TYPE;
    }
    public record OrderRejected(string Reason, DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Rejected";

        public string EventType => EVENT_TYPE;
    }
    public record OrderCancelled(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Cancelled";

        public string EventType => EVENT_TYPE;
    }

    public record OrderShipped(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Shipped";

        public string EventType => EVENT_TYPE;
    }

    public record OrderPaid(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Paid";

        public string EventType => EVENT_TYPE;
    }

    public record OrderUnpaid(DateTime TimeStamp) : IEvent
    {
        public static readonly string EVENT_TYPE = "Order_Unpaid";

        public string EventType => EVENT_TYPE;
    }
}