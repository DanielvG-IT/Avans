using Api.Models;

namespace Api.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that manages litter data.
    /// </summary>
    public interface ILitterRepository
    {
        /// <summary>
        /// Asynchronously adds a new litter to the repository.
        /// </summary>
        /// <param name="litter">The litter object to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(Litter litter);

        /// <summary>
        /// Asynchronously saves all changes made in the repository.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Asynchronously retrieves a list of litters based on the provided filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria to apply.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of <see cref="Litter"/> objects that match the filter.
        /// </returns>
        Task<List<Litter>> GetFilteredAsync(LitterFilterDto filter);
    }
}