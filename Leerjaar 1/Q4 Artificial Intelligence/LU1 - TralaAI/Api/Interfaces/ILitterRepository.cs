using Api.Models;
using Api.Models.Data;

namespace Api.Interfaces;

/// <summary>
/// Interface for managing litter-related data operations.
/// </summary>
public interface ILitterRepository
{
    /// <summary>
    /// Adds a new litter record asynchronously.
    /// </summary>
    /// <param name="litter">The litter entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Litter litter);

    /// <summary>
    /// Saves changes made to the repository asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveChangesAsync();

    /// <summary>
    /// Asynchronously retrieves a list of cameras.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="Camera"/> objects,
    /// or <c>null</c> if no cameras are found.
    /// </returns>
    Task<List<Camera>?> GetCamerasAsync();


    /// <summary>
    /// Retrieves a filtered list of litter records asynchronously based on the specified filter criteria.
    /// </summary>
    /// <param name="filter">The filter criteria for retrieving litter records.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of filtered litter records.</returns>
    Task<List<Litter>> GetFilteredAsync(LitterFilterDto filter);

    /// <summary>
    /// Retrieves the amount of litter per location asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing the amount of litter per location.</returns>
    Task<List<LitterAmountCamera>> GetAmountPerCameraAsync();
}