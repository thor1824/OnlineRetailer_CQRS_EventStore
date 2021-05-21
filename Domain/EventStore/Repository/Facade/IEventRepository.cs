using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using OnlineRetailer.Domain.Common;

namespace OnlineRetailer.Domain.EventStore.Repository.Facade
{
    public interface IEventRepository
    {
        Task<bool> ExistsAsync(string streamId);
        Task<List<ResolvedEvent>> GetAllByStreamIdAsync(string streamId);

        Task<List<ResolvedEvent>> GetAllAsync();

        Task ApplyAsync<TEvent>(TEvent evnt, string streamId) where TEvent : IEvent;
    }
}