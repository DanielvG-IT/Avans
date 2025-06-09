using Api.Models;

namespace Api.Interfaces
{
    public interface IAggregatedTrashService
    {
        Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync();
    }
}