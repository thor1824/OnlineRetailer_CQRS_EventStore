using System;

namespace OnlineRetailer.Domain.Exceptions
{
    public class EventStreamNotFound : Exception
    {
        public EventStreamNotFound(string streamId) : base($"EventStream: {streamId}, Not Found!")
        {
        }
    }
}