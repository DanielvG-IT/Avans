using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly string _connectionString;

    public PatientRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "SELECT * FROM Patients";
        return await connection.QueryAsync<Patient>(sqlQuery);
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "SELECT * FROM Patients WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Patient>(sqlQuery, new { Id = id });
    }

    public async Task<int> AddPatientAsync(Patient patient)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "INSERT INTO Patients (Name, Age, Address) VALUES (@Name, @Age, @Address)";
        return await connection.ExecuteAsync(sqlQuery, patient);
    }

    public async Task<int> UpdatePatientAsync(Patient patient)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "UPDATE Patients SET Name = @Name, Age = @Age, Address = @Address WHERE Id = @Id";
        return await connection.ExecuteAsync(sqlQuery, patient);
    }

    public async Task<int> DeletePatientAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "DELETE FROM Patients WHERE Id = @Id";
        return await connection.ExecuteAsync(sqlQuery, new { Id = id });
    }

    public async Task<IEnumerable<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "SELECT * FROM Patients WHERE DoctorID = @DoctorId";
        return await connection.QueryAsync<Patient>(sqlQuery, new { DoctorId = doctorId });
    }
}