using HQB.WebApi.Models;
using System.Collections.Generic;

namespace HQB.WebApi.Interfaces;
public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    Task<Appointment?> GetAppointmentByIdAsync(Guid id);
    Task<Appointment?> GetAppointmentByTreatmentIdAsync(Guid id);
    Task<int> AddAppointmentAsync(Appointment appointment);
    Task<int> UpdateAppointmentAsync(Appointment appointment);
    Task<int> DeleteAppointmentAsync(Guid id);
    // Task<Appointment?> GetAppointmentByPatientIdAsync(Guid id);
}