using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.EventStore.Streams
{
    public class CustomerStream : BaseStream
    {
        public const string STREAM_PREFIX = "customer-";

        public CustomerStream(Guid id) : base(id, STREAM_PREFIX)
        {
        }
    }
}