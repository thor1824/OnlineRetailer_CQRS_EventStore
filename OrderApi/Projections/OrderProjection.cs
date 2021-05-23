using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.OrderApi.Aggregates;

namespace OnlineRetailer.OrderApi.Projections
{
    public class OrderProjection : IProjection<Order>
    {
        public OrderProjection(BaseStream stream)
        {
            Stream = stream;
            Aggregate.OrderId = stream.AggregateId;
        }

        public BaseStream Stream { get; }
        public Order Aggregate { get; } = new();

        public void ApplyEvent(IEvent evnt)
        {
            Stream.AddEvent(evnt);
            switch (evnt)
            {
                case SubmitOrder submitOrder:
                    Apply(submitOrder);
                    break;
                case WaitingForCustomerValidation waitingForCustomerValidation:
                    Apply(waitingForCustomerValidation);
                    break;
                case CustomerValid customerValid:
                    Apply(customerValid);
                    break;
                case CustomerBadCredit customerBadCredit:
                    Apply(customerBadCredit);
                    break;
                case CustomerNotFound customerNotFound:
                    Apply(customerNotFound);
                    break;
                case WaitingForIsInStockValidation waitingForIsInStockValidation:
                    Apply(waitingForIsInStockValidation);
                    break;
                case AllItemsIsInStock allItemsIsInStock:
                    Apply(allItemsIsInStock);
                    break;
                case NotInStock notInStuck:
                    Apply(notInStuck);
                    break;
                case PartialInStock partialInStock:
                    Apply(partialInStock);
                    break;
                case OrderConfirmed orderConfirmed:
                    Apply(orderConfirmed);
                    break;
                case OrderRejected orderRejected:
                    Apply(orderRejected);
                    break;
                case OrderCancelled orderCancelled:
                    Apply(orderCancelled);
                    break;
                case OrderShipped orderShipped:
                    Apply(orderShipped);
                    break;
                case OrderPaid orderPaid:
                    Apply(orderPaid);
                    break;
                case OrderUnpaid orderUnpaid:
                    Apply(orderUnpaid);
                    break;
            }
        }

        private void Apply(CustomerNotFound _)
        {
            Aggregate.CustomerValidationStatus = CustomerValidationStatus.NotFound;
        }

        private void Apply(CustomerValid _)
        {
            Aggregate.CustomerValidationStatus = CustomerValidationStatus.CustomerValid;
        }

        private void Apply(CustomerBadCredit _)
        {
            Aggregate.CustomerValidationStatus = CustomerValidationStatus.BadCreditStanding;
        }

        private void Apply(AllItemsIsInStock _)
        {
            Aggregate.StockValidationStatus = StockValidationStatus.AllItemsIsInStock;
        }

        private void Apply(NotInStock _)
        {
            Aggregate.StockValidationStatus = StockValidationStatus.NotInStuck;
        }

        private void Apply(PartialInStock _)
        {
            Aggregate.StockValidationStatus = StockValidationStatus.PartialInStock;
        }

        private void Apply(OrderConfirmed _)
        {
            Aggregate.OrderStatus = OrderStatus.Confirmed;
        }

        private void Apply(OrderRejected orderCancelled)
        {
            Aggregate.OrderStatus = OrderStatus.Rejected;
            Aggregate.RejectionReason = orderCancelled.Reason;
        }

        private void Apply(OrderCancelled _)
        {
            Aggregate.OrderStatus = OrderStatus.Cancelled;
        }

        private void Apply(OrderShipped _)
        {
            Aggregate.OrderStatus = OrderStatus.Shipped;
        }

        private void Apply(OrderPaid _)
        {
            Aggregate.OrderStatus = OrderStatus.Unpaid;
        }

        private void Apply(OrderUnpaid _)
        {
            Aggregate.OrderStatus = OrderStatus.Unpaid;
        }

        private void Apply(WaitingForIsInStockValidation _)
        {
            Aggregate.StockValidationStatus = StockValidationStatus.WaitingForIsInStockValidation;
        }

        private void Apply(WaitingForCustomerValidation _)
        {
            Aggregate.CustomerValidationStatus = CustomerValidationStatus.WaitingForCustomerValidation;
        }

        private void Apply(SubmitOrder submitOrder)
        {
            Aggregate.CustomerId = submitOrder.CustomerId;
            Aggregate.Date = submitOrder.TimeStamp;
            Aggregate.OrderLines = submitOrder.OrderLines;
            Aggregate.OrderStatus = OrderStatus.Submitted;
        }
    }
}