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

    public async Task<IEnumerable<Appointment>> GetCompletedAppointmentsAsync()
    {
      using var connection = new SqlConnection(_connectionString);
      var query = "SELECT * FROM CompletedAppointments";
      return await connection.QueryAsync<Appointment>(query);
    }

    public async Task<Appointment?> GetCompletedAppointmentByIdAsync(Guid id)
    {
      using var connection = new SqlConnection(_connectionString);
      var query = "SELECT * FROM CompletedAppointments WHERE ID = @ID";
      return await connection.QueryFirstOrDefaultAsync<Appointment>(query, new { ID = id });
    }

    public async Task<IEnumerable<Appointment>> GetCompletedAppointmentsByPatientIdAsync(Guid patientId)
    {
      using var connection = new SqlConnection(_connectionString);
      var query = "SELECT * FROM CompletedAppointments WHERE PatientID = @PatientID";
      return await connection.QueryAsync<Appointment>(query, new { PatientID = patientId });
    }

    public async Task AddCompletedAppointmentAsync(Appointment appointment)
    {
      using var connection = new SqlConnection(_connectionString);
      var query = @"INSERT INTO CompletedAppointments (ID, PatientID, AppointmentID, CompletedDate, Notes)
                VALUES (@ID, @PatientID, @AppointmentID, @CompletedDate, @Notes)";
      await connection.ExecuteAsync(query, appointment);
    }

    public async Task UpdateCompletedAppointmentAsync(Appointment appointment)
    {
      using var connection = new SqlConnection(_connectionString);
      var query = @"UPDATE CompletedAppointments
                SET PatientID = @PatientID, AppointmentID = @AppointmentID, CompletedDate = @CompletedDate, Notes = @Notes
                WHERE ID = @ID";
      await connection.ExecuteAsync(query, appointment);
    }

    public async Task DeleteCompletedAppointmentAsync(Guid id)
    {
      using var connection = new SqlConnection(_connectionString);
      var query = "DELETE FROM CompletedAppointments WHERE ID = @ID";
      await connection.ExecuteAsync(query, new { ID = id });
    }
  }
}