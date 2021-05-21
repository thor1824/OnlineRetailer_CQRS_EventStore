using System;
using System.Collections.Generic;

namespace OnlineRetailer.Domain.Common
{
    public abstract class BaseStream
    {
        protected BaseStream(Guid id, string streamPrefix)
        {
            Id = id;
            StreamPrefix = streamPrefix;
        }

        public IList<IEvent> EventStream { get; } = new List<IEvent>();
        public Guid Id { get; }
        public string StreamPrefix { get; }
        public string StreamId => StreamPrefix + Id.ToString("N");

        public void AddEvent(IEvent evnt)
        {
            EventStream.Add(evnt);
        }
    }
}