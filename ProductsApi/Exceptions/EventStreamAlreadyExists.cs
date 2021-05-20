using System;

namespace OnlineRetailer.ProductsApi.Exceptions
{
    public class EventStreamAlreadyExists : Exception
    {
        public EventStreamAlreadyExists(string streamId) : base($"EventStream: {streamId}, Already Exists!")
        {
        }
    }
}