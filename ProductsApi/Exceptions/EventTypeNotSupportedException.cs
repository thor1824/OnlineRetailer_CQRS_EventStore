using System;

namespace OnlineRetailer.ProductsApi.Exceptions
{
    public class EventTypeNotSupportedException : Exception
    {
        public EventTypeNotSupportedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}