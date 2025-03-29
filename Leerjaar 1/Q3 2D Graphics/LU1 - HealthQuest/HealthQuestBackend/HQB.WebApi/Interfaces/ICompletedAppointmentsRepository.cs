using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for managing completed appointments.
/// </summary>
public interface ICompletedAppointmentsRepository
{
  /// <summary>
  /// Retrieves all completed appointments asynchronously.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains a collection of completed appointments.</returns>
  Task<IEnumerable<CompletedAppointment>> GetCompletedAppointmentsAsync();

  /// <summary>
  /// Retrieves a completed appointment by its identifier asynchronously.
  /// </summary>
  /// <param name="id">The identifier of the completed appointment.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the completed appointment.</returns>
  Task<CompletedAppointment?> GetCompletedAppointmentByIdAsync(Guid id);

  /// <summary>
  /// Retrieves all completed appointments for a specific patient asynchronously.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains a collection of completed appointments for the specified patient.</returns>
  Task<IEnumerable<Appointment>> GetCompletedAppointmentsByPatientIdAsync(Guid patientId);

  /// <summary>
  /// Adds a new completed appointment asynchronously.
  /// </summary>
  /// <param name="appointment">The completed appointment to add.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task AddCompletedAppointmentAsync(CompletedAppointment completedAppointment);

  /// <summary>
  /// Updates an existing completed appointment asynchronously.
  /// </summary>
  /// <param name="appointment">The completed appointment to update.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task UpdateCompletedAppointmentAsync(CompletedAppointment completedAppointment);

  /// <summary>
  /// Deletes a completed appointment by its identifier asynchronously.
  /// </summary>
  /// <param name="id">The identifier of the completed appointment to delete.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task DeleteCompletedAppointmentAsync(Guid id);
}