using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using OnlineRetailer.ProductsApi.Events.Facade;

namespace OnlineRetailer.ProductsApi.EventStore.Repository.Facade
{
    public interface IEventRepository
    {
        Task<bool> ExistsAsync(string streamId);
        Task<List<ResolvedEvent>> GetAllByStreamIdAsync(string streamId);

        Task<List<ResolvedEvent>> GetAllAsync();

        Task ApplyAsync<TEvent>(TEvent evnt, string streamId) where TEvent : IEvent;
    }
}