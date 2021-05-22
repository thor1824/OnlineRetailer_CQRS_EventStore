using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Streams
{
    public class OrderStream : BaseStream
    {
        public const string STREAM_PREFIX = "Order-";

        public OrderStream(Guid aggregateId) : base(aggregateId, STREAM_PREFIX)
        {
        }
    }
}