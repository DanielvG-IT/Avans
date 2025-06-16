using Api.Models;

/// <summary>
/// Defines the contract for a service that aggregates trash data.
/// </summary>
namespace Api.Interfaces
{
    public interface IAggregatedTrashService
    {
        /// <summary>
        /// Asynchronously retrieves a list of aggregated trash data.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="AggregatedTrashDto"/> objects,
        /// representing the aggregated trash data.
        /// </returns>
        Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync();
    }
}