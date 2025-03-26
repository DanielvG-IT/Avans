using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for patient repository to handle CRUD operations for patients.
/// </summary>
public interface IPatientRepository
{
    /// <summary>
    /// Retrieves all patients asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of patients.</returns>
    Task<IEnumerable<Patient>> GetAllPatientsAsync();

    /// <summary>
    /// Retrieves a patient by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the patient to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the patient if found; otherwise, null.</returns>
    Task<Patient?> GetPatientByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a patient associated with a specific doctor by the patient's ID.
    /// </summary>
    /// <param name="patientId">The ID of the patient to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the patient associated with the specified doctor, or null if no such patient exists.</returns>
    Task<IEnumerable<Patient>> GetPatientsByDoctorIdAsync(Guid patientId);

    /// <summary>
    /// Adds a new patient asynchronously.
    /// </summary>
    /// <param name="patient">The patient to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the newly added patient.</returns>
    Task<int> AddPatientAsync(Patient patient);

    /// <summary>
    /// Updates an existing patient asynchronously.
    /// </summary>
    /// <param name="patient">The patient with updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    Task<int> UpdatePatientAsync(Patient patient);

    /// <summary>
    /// Deletes a patient by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the patient to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of affected rows.</returns>
    Task<int> DeletePatientAsync(int id);
}