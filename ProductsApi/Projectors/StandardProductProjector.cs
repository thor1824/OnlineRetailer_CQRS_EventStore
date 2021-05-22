using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.ProductEvents;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.ProductsApi.Projections;

namespace OnlineRetailer.ProductsApi.Projectors
{
    public class StandardProductProjector : IProjector<ProductProjection>
    {
        public StandardProductProjector(BaseStream stream)
        {
            Stream = stream;
            Projection.Id = stream.AggregateId;
        }

        public BaseStream Stream { get; }

        public ProductProjection Projection { get; } = new();

        public void ApplyEvent(IEvent evnt)
        {
            Stream.AddEvent(evnt);
            switch (evnt)
            {
                case NewProduct addProduct:
                    Apply(addProduct);
                    break;
                case RemoveProduct removeProduct:
                    Apply(removeProduct);
                    break;
                case ChangeCategory changeCategory:
                    Apply(changeCategory);
                    break;
                case ChangeName changeName:
                    Apply(changeName);
                    break;
                case ChangePrice changePrice:
                    Apply(changePrice);
                    break;
                case Reserve reserve:
                    Apply(reserve);
                    break;
                case Restock restock:
                    Apply(restock);
                    break;
                default:
                    throw new EventTypeNotSupportedException($"Unsupported Event type detected: {evnt.GetType().Name}");
            }
        }

        private void Apply(NewProduct evnt)
        {
            Projection.Name = evnt.Name;
            Projection.Category = evnt.Category;
            Projection.Price = evnt.Price;
            Projection.ItemsInStock = evnt.ItemsInStock;
        }

        private void Apply(RemoveProduct evnt)
        {
            Projection.IsDeleted = true;
        }

        private void Apply(ChangeName evnt)
        {
            Projection.Name = evnt.NewName;
        }

        private void Apply(ChangePrice evnt)
        {
            Projection.Price = evnt.NewPrice;
        }

        private void Apply(ChangeCategory evnt)
        {
            Projection.Category = evnt.NewCategory;
        }

        private void Apply(Reserve evnt)
        {
            Projection.ItemsInStock -= evnt.AmountToReserved;
            Projection.ItemsReserved += evnt.AmountToReserved;
        }

        private void Apply(Restock evnt)
        {
            Projection.ItemsInStock += evnt.AmountRestock;
        }
    }
}