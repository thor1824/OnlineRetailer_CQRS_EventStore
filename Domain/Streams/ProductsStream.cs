using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.Streams
{
    public class ProductStream : BaseStream
    {
        public static readonly string STREAM_PREFIX = "product-";

        public ProductStream(Guid aggregateId) : base(aggregateId, STREAM_PREFIX)
        {
        }
    }
}