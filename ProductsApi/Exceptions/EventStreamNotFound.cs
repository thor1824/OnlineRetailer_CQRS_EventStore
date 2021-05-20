using System;

namespace OnlineRetailer.ProductsApi.Exceptions
{
    public class EventStreamNotFound : Exception
    {
        public EventStreamNotFound(string streamId) : base($"EventStream: {streamId}, Not Found!")
        {
        }
    }
}