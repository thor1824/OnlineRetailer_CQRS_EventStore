using System;

namespace OnlineRetailer.Domain.Exceptions
{
    public class EventStreamAlreadyExists : Exception
    {
        public EventStreamAlreadyExists(string streamId) : base($"EventStream: {streamId}, Already Exists!")
        {
        }
    }
}