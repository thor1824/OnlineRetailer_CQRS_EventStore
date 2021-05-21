using System;

namespace OnlineRetailer.Domain.Common
{
    public interface IEvent
    {
        public string EventType { get; }
        public DateTime TimeStamp { get; }
    }
}