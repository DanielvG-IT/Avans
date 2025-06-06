﻿using Dapper;
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
        const string sqlQuery = "SELECT * FROM Patient";
        return await connection.QueryAsync<Patient>(sqlQuery);
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sqlQuery = "SELECT * FROM Patient WHERE ID = @Id";
        return await connection.QueryFirstOrDefaultAsync<Patient>(sqlQuery, new { Id = id });
    }

    public async Task<IEnumerable<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sqlQuery = "SELECT * FROM Patient WHERE DoctorID = @DoctorId";
        return await connection.QueryAsync<Patient>(sqlQuery, new { DoctorId = doctorId });
    }

    public async Task<IEnumerable<Patient>> GetPatientsByGuardianId(Guid guardianId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sqlQuery = "SELECT * FROM Patient WHERE GuardianID = @GuardianId";
        return await connection.QueryAsync<Patient>(sqlQuery, new { GuardianId = guardianId });
    }

    public async Task<int> AddPatientAsync(Patient patient)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sqlQuery = "INSERT INTO Patient (ID, FirstName, LastName, GuardianID, TreatmentID, DoctorID, Avatar) VALUES (@Id, @FirstName, @LastName, @GuardianID, @TreatmentID, @DoctorID, @Avatar)";
        return await connection.ExecuteAsync(sqlQuery, patient);
    }

    public async Task<int> UpdatePatientAsync(Patient patient)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sqlQuery = "UPDATE Patient SET FirstName = @FirstName, LastName = @LastName, TreatmentID = @TreatmentID, DoctorID = @DoctorID, Avatar = @Avatar, GuardianAccessJournal = @GuardianAccessJournal, DoctorAccessJournal = @DoctorAccessJournal WHERE ID = @Id";
        return await connection.ExecuteAsync(sqlQuery, patient);
    }

    public async Task<int> DeletePatientAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string sqlQuery = "DELETE FROM Patient WHERE ID = @Id";
        return await connection.ExecuteAsync(sqlQuery, new { Id = id });
    }
}