using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.Events.ProductEvents;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.ProductsApi.Aggregates;

namespace OnlineRetailer.ProductsApi.Projections
{
    public class ProductProjection : IProjection<Product>
    {
        public ProductProjection(BaseStream stream)
        {
            Stream = stream;
            Aggregate.Id = stream.AggregateId;
        }

        public BaseStream Stream { get; }

        public Product Aggregate { get; } = new();

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
            Aggregate.Name = evnt.Name;
            Aggregate.Category = evnt.Category;
            Aggregate.Price = evnt.Price;
            Aggregate.ItemsInStock = evnt.ItemsInStock;
        }

        private void Apply(RemoveProduct evnt)
        {
            Aggregate.IsDeleted = true;
        }

        private void Apply(ChangeName evnt)
        {
            Aggregate.Name = evnt.NewName;
        }

        private void Apply(ChangePrice evnt)
        {
            Aggregate.Price = evnt.NewPrice;
        }

        private void Apply(ChangeCategory evnt)
        {
            Aggregate.Category = evnt.NewCategory;
        }

        private void Apply(Reserve evnt)
        {
            Aggregate.ItemsInStock -= evnt.AmountToReserved;
            Aggregate.ItemsReserved += evnt.AmountToReserved;
        }

        private void Apply(Restock evnt)
        {
            Aggregate.ItemsInStock += evnt.AmountRestock;
        }
    }
}