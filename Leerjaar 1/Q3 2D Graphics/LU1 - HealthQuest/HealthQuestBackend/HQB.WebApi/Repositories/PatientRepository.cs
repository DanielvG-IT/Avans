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
        string sqlQuery = "SELECT * FROM Patients WHERE ID = @Id";
        return await connection.QueryFirstOrDefaultAsync<Patient>(sqlQuery, new { Id = id });
    }

    public async Task<IEnumerable<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "SELECT * FROM Patients WHERE DoctorID = @DoctorId";
        return await connection.QueryAsync<Patient>(sqlQuery, new { DoctorId = doctorId });
    }

    public async Task<IEnumerable<Patient>> GetPatientsByGuardianId(Guid guardianId)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "SELECT * FROM Patients WHERE GuardianID = @GuardianId";
        return await connection.QueryAsync<Patient>(sqlQuery, new { GuardianId = guardianId });
    }

    public async Task<int> AddPatientAsync(Patient patient)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "INSERT INTO Patients (FirstName, LastName, GuardianID, TreatmentID, DoctorID, Avatar) VALUES (@FirstName, @LastName, @GuardianID, @TreatmentID, @DoctorID, @Avatar)";
        return await connection.ExecuteAsync(sqlQuery, patient);
    }

    public async Task<int> UpdatePatientAsync(Patient patient)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "UPDATE Patients SET FirstName = @FirstName, LastName = @LastName, GuardianID = @GuardianID, TreatmentID = @TreatmentID, DoctorID = @DoctorID, Avatar = @Avatar WHERE ID = @Id";
        return await connection.ExecuteAsync(sqlQuery, patient);
    }

    public async Task<int> DeletePatientAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        string sqlQuery = "DELETE FROM Patients WHERE ID = @Id";
        return await connection.ExecuteAsync(sqlQuery, new { Id = id });
    }
}