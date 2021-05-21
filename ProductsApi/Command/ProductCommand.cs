using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Common;
using OnlineRetailer.Domain.EventStore.Repository.Facade;
using OnlineRetailer.Domain.EventStore.Streams;
using OnlineRetailer.Domain.Exceptions;
using OnlineRetailer.ProductsApi.Command.Facades;
using OnlineRetailer.ProductsApi.Events;

namespace OnlineRetailer.ProductsApi.Command
{
    public class ProductCommand : IProductCommand
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<ProductCommand> _logger;

        public ProductCommand(ILogger<ProductCommand> logger, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
        }

        public async Task<(bool wasSucces, string message, Guid guid)> AddAsync(string name, string category,
            decimal price,
            int itemsInStock)
        {
            var newId = Guid.NewGuid();
            _logger.Log(LogLevel.Debug, $"Product was assigned: id: {newId.ToString()}," +
                                        $"Name: {name}, Category: {category}, Price: {price}, Stock: {itemsInStock}");

            var evnt = new NewProduct(name, category, price, itemsInStock, DateTime.UtcNow);

            var (wasSuccess, message) = await ApplyNewAsync(newId, evnt);
            return (wasSuccess, message, newId);
        }

        public async Task<(bool wasSucces, string message)> RemoveAsync(Guid id)
        {
            _logger.Log(LogLevel.Debug, $"Removing Product: {id}");
            var evnt = new RemoveProduct(DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }


        public async Task<(bool wasSucces, string message)> IncreaseStuckAsync(Guid id, int amountRestocked)
        {
            _logger.Log(LogLevel.Debug, $"Restocking Product: {id}, with {amountRestocked} new items");
            var evnt = new Restock(amountRestocked, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message)> ReserveStuckAsync(Guid id, int amountReserved)
        {
            _logger.Log(LogLevel.Debug, $"Reserving Product: {id}. {amountReserved} was reserved");
            var evnt = new Reserve(amountReserved, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }


        public async Task<(bool wasSucces, string message)> RenameAsync(Guid id, string name)
        {
            _logger.Log(LogLevel.Debug, $"Altering Product: {id}. name was change to {name}");
            var evnt = new ChangeName(name, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        public async Task<(bool wasSucces, string message)> ChangeCategoryAsync(Guid id, string category)
        {
            _logger.Log(LogLevel.Debug, $"Altering Product: {id}. category was change to {category}");
            var evnt = new ChangeCategory(category, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }


        public async Task<(bool wasSucces, string message)> ChangePriceAsync(Guid id, decimal price)
        {
            _logger.Log(LogLevel.Debug, $"Altering Product: {id}. Price was change to {price}");
            var evnt = new ChangePrice(price, DateTime.UtcNow);

            return await ApplyChangeAsync(id, evnt);
        }

        private async Task<(bool wasSucces, string message)> ApplyChangeAsync<TEvent>(Guid id, TEvent evnt)
            where TEvent : IEvent
        {
            var productStream = new ProductStream(id);
            var exists = await _eventRepository.ExistsAsync(productStream.StreamId);
            if (!exists) throw new EventStreamNotFound(productStream.StreamId);

            await _eventRepository.ApplyAsync(evnt, productStream.StreamId);

            return (true, "Success");
        }

        private async Task<(bool wasSucces, string message)> ApplyNewAsync<TEvent>(Guid id, TEvent evnt)
            where TEvent : IEvent
        {
            var productStream = new ProductStream(id);

            if (await _eventRepository.ExistsAsync(productStream.StreamId))
                throw new EventStreamAlreadyExists(productStream.StreamId);

            await _eventRepository.ApplyAsync(evnt, productStream.StreamId);

            return (true, "Success");
        }
    }
}