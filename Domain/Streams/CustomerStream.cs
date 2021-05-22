using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Streams
{
    public class CustomerStream : BaseStream
    {
        public const string STREAM_PREFIX = "customer-";

        public CustomerStream(Guid aggregateId) : base(aggregateId, STREAM_PREFIX)
        {
        }
    }
}