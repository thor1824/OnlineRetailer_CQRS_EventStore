using System;
using System.Collections.Generic;
using OnlineRetailer.ProductsApi.Events;
using OnlineRetailer.ProductsApi.Events.Facade;
using OnlineRetailer.ProductsApi.Exceptions;
using OnlineRetailer.ProductsApi.Models.Projections;

namespace OnlineRetailer.ProductsApi.Models
{
    public abstract class BaseStream<TEventInterFace, TProjection> where TEventInterFace : IEvent
    {
        public IProjector<TEventInterFace, TProjection> Projector { get; }
        

        protected BaseStream(Guid id, string streamPrefix, IProjector<TEventInterFace, TProjection> projector)
        {
            Id = id;
            StreamPrefix = streamPrefix;
            Projector = projector;
        }

        public IList<TEventInterFace> EventStream { get; } = new List<TEventInterFace>();
        public Guid Id { get; }
        public string StreamPrefix { get; }
        public string StreamId => StreamPrefix + Id.ToString("N");

        public void ApplyEvent(TEventInterFace evnt)
        {
            EventStream.Add(evnt);
            Projector.ApplyEvent(evnt);
        }
        
    }

    public interface IProjector<TEventInterFace, TProjection> where TEventInterFace : IEvent
    {
        TProjection Projection { get; }

        void ApplyEvent(TEventInterFace evnt);

        TProjection GetProjection();
    }

    public class StandardProductProjector : IProjector<IProductEvent, ProductProjection>
    {
        public ProductProjection Projection { get; }

        public void ApplyEvent(IProductEvent evnt)
        {
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

        public ProductProjection GetProjection()
        {
            return Projection;
        }
    }

    public interface IProductEvent : IEvent
    {
    }

    public class ProductsStream<TProjection> : BaseStream<IProductEvent, TProjection>
    {
        public ProductsStream(Guid id, IProjector<IProductEvent, TProjection> projector) : base(id, "product-", projector)
        {
            
        }

        public ProductsStream(Guid id): this(id, null)
        {
        }
    }

    public class ProductStream 
    {
        private readonly ProductProjection _currentState = new();
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