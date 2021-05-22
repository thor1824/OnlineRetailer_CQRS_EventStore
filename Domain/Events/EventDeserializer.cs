using System.Text.Json;
using EventStore.Client;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.CustomerEvents;
using OnlineRetailer.Domain.Events.OrderEvents;
using OnlineRetailer.Domain.Events.ProductEvents;
using OnlineRetailer.Domain.Exceptions;
using ChangeName = OnlineRetailer.Domain.Events.CustomerEvents.ChangeName;

namespace OnlineRetailer.Domain.Events
{
    public static class  EventDeserializer
    {
        public static IEvent DeserializeCustomerEvent(EventRecord record)
        {
            if (record.EventType == NewCustomer.EVENT_TYPE)
                return JsonSerializer.Deserialize<NewCustomer>(record.Data.ToArray());

            if (record.EventType == RemoveCustomer.EVENT_TYPE)
                return JsonSerializer.Deserialize<RemoveCustomer>(record.Data.ToArray());

            if (record.EventType == ChangeName.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeName>(record.Data.ToArray());

            if (record.EventType == ChangeEmail.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeEmail>(record.Data.ToArray());

            if (record.EventType == ChangePhone.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangePhone>(record.Data.ToArray());

            if (record.EventType == ChangeShippingAddress.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeShippingAddress>(record.Data.ToArray());

            if (record.EventType == ChangeBillingAddress.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeBillingAddress>(record.Data.ToArray());

            if (record.EventType == ChangeCreditStanding.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeCreditStanding>(record.Data.ToArray());

            throw new EventTypeNotSupportedException($"Event of type {record.EventType}, not supported");
        }
        
        public static IEvent DeserializeOrderEvent(EventRecord record)
        {
            if (record.EventType == SubmitOrder.EVENT_TYPE)
                return JsonSerializer.Deserialize<SubmitOrder>(record.Data.ToArray());
            
            if (record.EventType == WaitingForCustomerValidation.EVENT_TYPE)
                return JsonSerializer.Deserialize<WaitingForCustomerValidation>(record.Data.ToArray());
            
            if (record.EventType == CustomerValid.EVENT_TYPE)
                return JsonSerializer.Deserialize<CustomerValid>(record.Data.ToArray());
            
            if (record.EventType == CustomerBadCredit.EVENT_TYPE)
                return JsonSerializer.Deserialize<CustomerBadCredit>(record.Data.ToArray());
            
            if (record.EventType == CustomerNotFound.EVENT_TYPE)
                return JsonSerializer.Deserialize<CustomerNotFound>(record.Data.ToArray());
            
            if (record.EventType == WaitingForIsInStockValidation.EVENT_TYPE)
                return JsonSerializer.Deserialize<WaitingForIsInStockValidation>(record.Data.ToArray());
            
            if (record.EventType == AllItemsIsInStock.EVENT_TYPE)
                return JsonSerializer.Deserialize<AllItemsIsInStock>(record.Data.ToArray());
            
            if (record.EventType == NotInStock.EVENT_TYPE)
                return JsonSerializer.Deserialize<NotInStock>(record.Data.ToArray());
            
            if (record.EventType == DoesNotExist.EVENT_TYPE)
                return JsonSerializer.Deserialize<DoesNotExist>(record.Data.ToArray());
            
            if (record.EventType == PartialInStock.EVENT_TYPE)
                return JsonSerializer.Deserialize<PartialInStock>(record.Data.ToArray());
            
            if (record.EventType == OrderConfirmed.EVENT_TYPE)
                return JsonSerializer.Deserialize<OrderConfirmed>(record.Data.ToArray());
            
            if (record.EventType == OrderRejected.EVENT_TYPE)
                return JsonSerializer.Deserialize<OrderRejected>(record.Data.ToArray());
            
            if (record.EventType == OrderCancelled.EVENT_TYPE)
                return JsonSerializer.Deserialize<OrderCancelled>(record.Data.ToArray());
            
            if (record.EventType == OrderShipped.EVENT_TYPE)
                return JsonSerializer.Deserialize<OrderShipped>(record.Data.ToArray());
            
            if (record.EventType == OrderPaid.EVENT_TYPE)
                return JsonSerializer.Deserialize<WaitingForCustomerValidation>(record.Data.ToArray());
            
            if (record.EventType == OrderUnpaid.EVENT_TYPE)
                return JsonSerializer.Deserialize<OrderUnpaid>(record.Data.ToArray());

            throw new EventTypeNotSupportedException($"Event of type {record.EventType}, not supported");
        }
        
        public static IEvent DeserializeProductEvent(EventRecord record)
        {
            if (record.EventType == NewProduct.EVENT_TYPE)
                return JsonSerializer.Deserialize<NewProduct>(record.Data.ToArray());

            if (record.EventType == RemoveProduct.EVENT_TYPE)
                return JsonSerializer.Deserialize<RemoveProduct>(record.Data.ToArray());

            if (record.EventType == ChangeName.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeName>(record.Data.ToArray());

            if (record.EventType == ChangeCategory.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangeCategory>(record.Data.ToArray());

            if (record.EventType == ChangePrice.EVENT_TYPE)
                return JsonSerializer.Deserialize<ChangePrice>(record.Data.ToArray());

            if (record.EventType == Restock.EVENT_TYPE)
                return JsonSerializer.Deserialize<Restock>(record.Data.ToArray());

            if (record.EventType == Reserve.EVENT_TYPE)
                return JsonSerializer.Deserialize<Reserve>(record.Data.ToArray());

            throw new EventTypeNotSupportedException($"Event of type {record.EventType}, not supported");
        }
    }
}