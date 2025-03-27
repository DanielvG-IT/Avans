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
        var sql = "SELECT * FROM Appoinment";
        return await connection.QueryAsync<Appointment>(sql);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Appoinment WHERE ID = @ID";
        return await connection.QueryFirstOrDefaultAsync<Appointment>(sql, new { ID = id });
    }

    public async Task<Appointment?> GetAppointmentByTreatmentIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Treatment_Appoinment WHERE TreatmentID = @TreatmentID";
        return await connection.QueryFirstOrDefaultAsync<Appointment>(sql, new { TreatmentID = id });
    }

    public async Task<int> AddAppointmentAsync(Appointment appointment)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "INSERT INTO Appoinment (ID, Name, Url, Image, DurationInMinutes) VALUES (@ID, @Name, @Url, @Image, @DurationInMinutes)";
        return await connection.ExecuteAsync(sql, appointment);
    }

    public async Task<int> UpdateAppointmentAsync(Appointment appointment)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "UPDATE Appoinment SET Name = @Name, Url = @Url, Image = @Image, DurationInMinutes = @DurationInMinutes WHERE ID = @ID";
        return await connection.ExecuteAsync(sql, appointment);
    }

    public async Task<int> DeleteAppointmentAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "DELETE FROM Appoinment WHERE ID = @ID";
        return await connection.ExecuteAsync(sql, new { ID = id });
    }
}