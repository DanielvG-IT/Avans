using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for managing appointments in the repository.
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>
    /// Retrieves all appointments asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of appointments.</returns>
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();

    /// <summary>
    /// Retrieves an appointment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the appointment if found; otherwise, null.</returns>
    Task<Appointment?> GetAppointmentByIdAsync(Guid id);

    /// <summary>
    /// Retrieves an appointment by its treatment identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the treatment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the appointment if found; otherwise, null.</returns>
    Task<IEnumerable<TreatmentAppointment>> GetAppointmentsByTreatmentIdAsync(Guid treatmentId);

    /// <summary>
    /// Adds a new appointment asynchronously.
    /// </summary>
    /// <param name="appointment">The appointment to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> AddAppointmentAsync(Appointment appointment);

    /// <summary>
    /// Updates an existing appointment asynchronously.
    /// </summary>
    /// <param name="appointment">The appointment to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> UpdateAppointmentAsync(Appointment appointment);

    /// <summary>
    /// Deletes an appointment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the appointment to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> DeleteAppointmentAsync(Guid id);
}