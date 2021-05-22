using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.OrderApi.Projections;

namespace OnlineRetailer.OrderApi.Projectors
{
    public class OrderProjector : IProjector<OrderProjection>
    {
        public OrderProjector(BaseStream stream)
        {
            Stream = stream;
            Projection.OrderId = stream.AggregateId;
        }

        public BaseStream Stream { get; }
        public OrderProjection Projection { get; } = new();

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
            Projection.CustomerValidationStatus = CustomerValidationStatus.NotFound;
        }

        private void Apply(CustomerValid _)
        {
            Projection.CustomerValidationStatus = CustomerValidationStatus.CustomerValid;
        }

        private void Apply(CustomerBadCredit _)
        {
            Projection.CustomerValidationStatus = CustomerValidationStatus.BadCreditStanding;
        }

        private void Apply(AllItemsIsInStock _)
        {
            Projection.StockValidationStatus = StockValidationStatus.AllItemsIsInStock;
        }

        private void Apply(NotInStock _)
        {
            Projection.StockValidationStatus = StockValidationStatus.NotInStuck;
        }

        private void Apply(PartialInStock _)
        {
            Projection.StockValidationStatus = StockValidationStatus.PartialInStock;
        }

        private void Apply(OrderConfirmed _)
        {
            Projection.OrderStatus = OrderStatus.Confirmed;
        }

        private void Apply(OrderRejected orderCancelled)
        {
            Projection.OrderStatus = OrderStatus.Rejected;
            Projection.RejectionReason = orderCancelled.Reason;
        }

        private void Apply(OrderCancelled _)
        {
            Projection.OrderStatus = OrderStatus.Cancelled;
        }

        private void Apply(OrderShipped _)
        {
            Projection.OrderStatus = OrderStatus.Shipped;
        }

        private void Apply(OrderPaid _)
        {
            Projection.OrderStatus = OrderStatus.Unpaid;
        }

        private void Apply(OrderUnpaid _)
        {
            Projection.OrderStatus = OrderStatus.Unpaid;
        }

        private void Apply(WaitingForIsInStockValidation _)
        {
            Projection.StockValidationStatus = StockValidationStatus.WaitingForIsInStockValidation;
        }

        private void Apply(WaitingForCustomerValidation _)
        {
            Projection.CustomerValidationStatus = CustomerValidationStatus.WaitingForCustomerValidation;
        }

        private void Apply(SubmitOrder submitOrder)
        {
            Projection.CustomerId = submitOrder.CustomerId;
            Projection.Date = submitOrder.TimeStamp;
            Projection.OrderLines = submitOrder.OrderLines;
            Projection.OrderStatus = OrderStatus.Submitted;
        }
    }
}