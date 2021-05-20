using System;
using System.Collections.Generic;
using OnlineRetailer.ProductsApi.Events;
using OnlineRetailer.ProductsApi.Events.Facade;
using OnlineRetailer.ProductsApi.Exceptions;
using OnlineRetailer.ProductsApi.Models.Projections;

namespace OnlineRetailer.ProductsApi.Models
{
    public class ProductStream
    {
        private readonly ProductProjection _currentState = new();
        private readonly IList<IEvent> _events = new List<IEvent>();
        private readonly string _streamPrefix = "product-";

        public ProductStream(Guid id)
        {
            Id = id;
            _currentState.Id = id;
        }

        public Guid Id { get; }
        public string StreamId => _streamPrefix + Id.ToString("N");

        public void ApplyEvent(IEvent evnt)
        {
            _currentState.LastModified = evnt.TimeStamp;
            _events.Add(evnt);
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

        public IList<IEvent> GetEvents()
        {
            return _events;
        }

        #region Projections

        public ProductProjection ProjectProduct()
        {
            return _currentState;
        }

        #endregion

        #region Apply overloads

        private void Apply(NewProduct evnt)
        {
            _currentState.Name = evnt.Name;
            _currentState.Category = evnt.Category;
            _currentState.Price = evnt.Price;
            _currentState.ItemsInStock = evnt.ItemsInStock;
        }

        private void Apply(RemoveProduct evnt)
        {
            _currentState.IsDeleted = true;
        }

        private void Apply(ChangeName evnt)
        {
            _currentState.Name = evnt.NewName;
        }

        private void Apply(ChangePrice evnt)
        {
            _currentState.Price = evnt.NewPrice;
        }

        private void Apply(ChangeCategory evnt)
        {
            _currentState.Category = evnt.NewCategory;
        }

        private void Apply(Reserve evnt)
        {
            _currentState.ItemsInStock -= evnt.AmountToReserved;
            _currentState.ItemsReserved += evnt.AmountToReserved;
        }

        private void Apply(Restock evnt)
        {
            _currentState.ItemsInStock += evnt.AmountRestock;
        }

        #endregion

        /*public void NewProduct(string name)
        {
            Apply(new NewProduct(Id, name, DateTime.UtcNow));
        }*/
    }
}