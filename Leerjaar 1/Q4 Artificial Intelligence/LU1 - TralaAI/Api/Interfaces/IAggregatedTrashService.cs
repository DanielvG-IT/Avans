
using TralaAI.CoreApi.Models;

namespace TralaAI.CoreApi.Interfaces
{
    public interface IAggregatedTrashService
    {
        Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync();
    }
}