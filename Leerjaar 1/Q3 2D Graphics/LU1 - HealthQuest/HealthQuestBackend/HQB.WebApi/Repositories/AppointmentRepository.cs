using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories;
public class AppointmentRepository : IAppointmentRepository
{
    private readonly string _connectionString;
    public AppointmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Appoinment";
        return await connection.QueryAsync<Appointment>(sql);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Appointment WHERE ID = @ID";
        return await connection.QueryFirstOrDefaultAsync<Appointment>(sql, new { ID = id });
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByTreatmentIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Appointment WHERE TreatmentID = @TreatmentID";
        return await connection.QueryAsync<Appointment>(sql, new { TreatmentID = id });
    }

    public async Task<int> AddAppointmentAsync(Appointment appointment)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "INSERT INTO Appointment (ID, Name, Url, Description, Image, DurationInMinutes, TreatmentID, Sequence) VALUES (@ID, @Name, @Url, @Description, @Image, @DurationInMinutes, @TreatmentID, @Sequence)";
        return await connection.ExecuteAsync(sql, appointment);
    }

    public async Task<int> UpdateAppointmentAsync(Appointment appointment)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "UPDATE Appointment SET Name = @Name, Url = @Url, Description = @Description, Image = @Image, DurationInMinutes = @DurationInMinutes WHERE ID = @ID, TreatmentID = @TreatmentID, Sequence = @Sequence";
        return await connection.ExecuteAsync(sql, appointment);
    }

    public async Task<int> DeleteAppointmentAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sql = "DELETE FROM Appointment WHERE ID = @ID";
        return await connection.ExecuteAsync(sql, new { ID = id });
    }
}