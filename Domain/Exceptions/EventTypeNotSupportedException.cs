using System;

namespace OnlineRetailer.Domain.Exceptions
{
    public class EventTypeNotSupportedException : Exception
    {
        public EventTypeNotSupportedException(string errorMessage) : base(errorMessage)
        {
        }
    }
}