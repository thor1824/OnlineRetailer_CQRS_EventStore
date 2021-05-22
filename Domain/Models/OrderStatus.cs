namespace OnlineRetailer.Domain.Models
{
    public enum OrderStatus
    {
        Submitted,
        Confirmed,
        Rejected,
        Cancelled,
        Shipped,
        Paid,
        Unpaid
    }

    public enum StockValidationStatus
    {
        WaitingForIsInStockValidation,
        AllItemsIsInStock,
        PartialInStock,
        ProductDoesNotExists,
        NotInStuck
    }

    public enum CustomerValidationStatus
    {
        WaitingForCustomerValidation,
        CustomerValid,
        NotFound,
        BadCreditStanding
    }
}