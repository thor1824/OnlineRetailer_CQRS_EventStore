using System;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.EventStore.Streams
{
    public class ProductStream : BaseStream
    {
        public static readonly string STREAM_PREFIX = "product-";

        public ProductStream(Guid id) : base(id, STREAM_PREFIX)
        {
        }
    }
}