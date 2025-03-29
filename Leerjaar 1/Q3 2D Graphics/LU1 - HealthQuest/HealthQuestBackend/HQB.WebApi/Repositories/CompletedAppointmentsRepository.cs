using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories
{
  public class CompletedAppointmentsRepository : ICompletedAppointmentsRepository
  {
    private readonly string _connectionString;

    public CompletedAppointmentsRepository(string connectionString)
    {
      _connectionString = connectionString;
    }

    public async Task<IEnumerable<CompletedAppointment>> GetCompletedAppointmentsAsync()
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"
      SELECT 
        ca.ID, 
        ca.PatientID, 
        ca.AppointmentID, 
        ca.CompletedDate, 
        a.Name AS AppointmentName, 
        a.Url AS AppointmentUrl, 
        a.Image AS AppointmentImage, 
        a.DurationInMinutes AS AppointmentDurationInMinutes
      FROM CompletedAppointments ca
      INNER JOIN Appoinment a ON ca.AppointmentID = a.ID";
      return await connection.QueryAsync<CompletedAppointment>(query);
    }

    public async Task<CompletedAppointment?> GetCompletedAppointmentByIdAsync(Guid id)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"
      SELECT 
        ca.ID, 
        ca.PatientID, 
        ca.AppointmentID, 
        ca.CompletedDate, 
        a.Name AS AppointmentName, 
        a.Url AS AppointmentUrl, 
        a.Image AS AppointmentImage, 
        a.DurationInMinutes AS AppointmentDurationInMinutes
      FROM CompletedAppointments ca
      INNER JOIN Appoinment a ON ca.AppointmentID = a.ID
      WHERE ca.ID = @ID";
      return await connection.QueryFirstOrDefaultAsync<CompletedAppointment>(query, new { ID = id });
    }

    public async Task<IEnumerable<Appointment>> GetCompletedAppointmentsByPatientIdAsync(Guid patientId)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"
      SELECT 
      a.ID, 
      a.Name, 
      a.Url, 
      a.Image, 
      a.DurationInMinutes
      FROM CompletedAppointments ca
      INNER JOIN Appoinment a ON ca.AppointmentID = a.ID
      WHERE ca.PatientID = @PatientID";
      return await connection.QueryAsync<Appointment>(query, new { PatientID = patientId });
    }

    public async Task AddCompletedAppointmentAsync(CompletedAppointment completedAppointment)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"INSERT INTO CompletedAppointments (ID, PatientID, AppointmentID, CompletedDate) VALUES (@ID, @PatientID, @AppointmentID, @CompletedDate)";
      await connection.ExecuteAsync(query, completedAppointment);
    }

    public async Task UpdateCompletedAppointmentAsync(CompletedAppointment completedAppointment)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = @"UPDATE CompletedAppointments SET PatientID = @PatientID, AppointmentID = @AppointmentID, CompletedDate = @CompletedDate WHERE ID = @ID";
      await connection.ExecuteAsync(query, completedAppointment);
    }

    public async Task DeleteCompletedAppointmentAsync(Guid id)
    {
      using var connection = new SqlConnection(_connectionString);
      const string query = "DELETE FROM CompletedAppointments WHERE ID = @ID";
      await connection.ExecuteAsync(query, new { ID = id });
    }
  }
}