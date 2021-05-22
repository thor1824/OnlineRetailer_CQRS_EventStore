using System;
using System.Collections.Generic;

namespace OnlineRetailer.Domain.Common
{
    public abstract class BaseStream
    {
        protected BaseStream(Guid aggregateId, string streamPrefix)
        {
            AggregateId = aggregateId;
            StreamPrefix = streamPrefix;
        }

        public IList<IEvent> EventStream { get; } = new List<IEvent>();
        public Guid AggregateId { get; }
        public string StreamPrefix { get; }
        public string StreamId => StreamPrefix + AggregateId.ToString("N");

        public void AddEvent(IEvent evnt)
        {
            EventStream.Add(evnt);
        }
    }
}