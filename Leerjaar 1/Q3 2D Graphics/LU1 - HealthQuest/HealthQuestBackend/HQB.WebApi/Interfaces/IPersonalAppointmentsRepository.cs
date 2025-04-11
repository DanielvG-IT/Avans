using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for managing personal appointments in the system.
/// </summary>
public interface IPersonalAppointmentsRepository
{
    /// <summary>
    /// Retrieves all personal appointments.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of personal appointments.</returns>
    Task<IEnumerable<PersonalAppointments>> GetPersonalAppointments();

    /// <summary>
    /// Retrieves personal appointments for a specific patient by their ID.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of personal appointments for the specified patient.</returns>
    Task<IEnumerable<PersonalAppointments>> GetPersonalAppointmentsByPatientId(Guid patientId);

    /// <summary>
    /// Retrieves a specific personal appointment by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the personal appointment.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the personal appointment if found; otherwise, null.</returns>
    Task<PersonalAppointments?> GetPersonalAppointmentById(Guid id);

    /// <summary>
    /// Adds a new personal appointment to the system.
    /// </summary>
    /// <param name="personalAppointment">The personal appointment to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
    Task<int> AddPersonalAppointment(PersonalAppointments personalAppointment);

    /// <summary>
    /// Updates an existing personal appointment in the system.
    /// </summary>
    /// <param name="personalAppointment">The personal appointment with updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
    Task<int> UpdatePersonalAppointment(PersonalAppointments personalAppointment);

    /// <summary>
    /// Deletes a personal appointment from the system by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the personal appointment to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
    Task<int> DeletePersonalAppointment(Guid id);

    /// <summary>
    /// Marks a personal appointment as completed.
    /// </summary>
    /// <param name="id">The unique identifier of the personal appointment to mark as completed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
    Task<int> MarkAppointmentAsCompleted(Guid id);

    /// <summary>
    /// Marks a question associated with a personal appointment as completed.
    /// </summary>
    /// <param name="id">The unique identifier of the question to mark as completed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
    Task<int> MarkQuestionAsCompleted(Guid id);
}