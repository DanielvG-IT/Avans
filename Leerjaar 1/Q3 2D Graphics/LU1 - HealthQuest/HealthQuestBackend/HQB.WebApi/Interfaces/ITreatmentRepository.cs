using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for managing treatments in the repository.
/// </summary>
public interface ITreatmentRepository
{
    /// <summary>
    /// Retrieves all treatments asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of treatments.</returns>
    Task<IEnumerable<Treatment>> GetAllTreatmentsAsync();

    /// <summary>
    /// Retrieves a treatment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the treatment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the treatment if found; otherwise, null.</returns>
    Task<Treatment?> GetTreatmentByIdAsync(Guid id);

    /// <summary>
    /// Adds a new treatment asynchronously.
    /// </summary>
    /// <param name="treatment">The treatment to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> AddTreatmentAsync(Treatment treatment);

    /// <summary>
    /// Updates an existing treatment asynchronously.
    /// </summary>
    /// <param name="treatment">The treatment to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> UpdateTreatmentAsync(Treatment treatment);

    /// <summary>
    /// Deletes a treatment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the treatment to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> DeleteTreatmentAsync(Guid id);
}