using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for Guardian repository to handle CRUD operations.
/// </summary>
public interface IGuardianRepository
{
    /// <summary>
    /// Retrieves all guardians asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of guardians.</returns>
    Task<IEnumerable<Guardian>> GetAllGuardiansAsync();

    /// <summary>
    /// Retrieves a guardian by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the guardian.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the guardian if found; otherwise, null.</returns>
    Task<Guardian?> GetGuardianByIdAsync(Guid id);

    /// <summary>
    /// Adds a new guardian asynchronously.
    /// </summary>
    /// <param name="guardian">The guardian to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> AddGuardianAsync(Guardian guardian);

    /// <summary>
    /// Updates an existing guardian asynchronously.
    /// </summary>
    /// <param name="guardian">The guardian to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> UpdateGuardianAsync(Guardian guardian);

    /// <summary>
    /// Deletes a guardian by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the guardian to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> DeleteGuardianAsync(Guid id);
}