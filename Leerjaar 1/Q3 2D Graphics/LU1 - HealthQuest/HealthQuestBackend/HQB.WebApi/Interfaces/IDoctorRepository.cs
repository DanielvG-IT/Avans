using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for Doctor repository to handle CRUD operations for Doctor entities.
/// </summary>
public interface IDoctorRepository
{
    /// <summary>
    /// Retrieves all doctors asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of doctors.</returns>
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync();

    /// <summary>
    /// Retrieves a doctor by their unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the doctor.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the doctor entity if found; otherwise, null.</returns>
    Task<Doctor?> GetDoctorByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a doctor by their UserID asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user linked to a doctor.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the doctor entity if found; otherwise, null.</returns>
    Task<Doctor?> GetDoctorByUserIDAsync(Guid userId);

    /// <summary>
    /// Adds a new doctor asynchronously.
    /// </summary>
    /// <param name="doctor">The doctor entity to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> AddDoctorAsync(Doctor doctor);

    /// <summary>
    /// Updates an existing doctor asynchronously.
    /// </summary>
    /// <param name="doctor">The doctor entity to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> UpdateDoctorAsync(Doctor doctor);

    /// <summary>
    /// Deletes a doctor by their unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the doctor to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> DeleteDoctorAsync(Guid id);
}