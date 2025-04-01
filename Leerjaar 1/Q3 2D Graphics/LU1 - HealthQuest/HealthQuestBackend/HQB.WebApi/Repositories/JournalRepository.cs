using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories;
public class JournalRepository : IJournalRepository
{
    private readonly string _connectionString;

    public JournalRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<JournalEntry>> GetAllJournalEntriesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = "SELECT * FROM JournalEntry";
        return await connection.QueryAsync<JournalEntry>(query);
    }

    public async Task<JournalEntry?> GetJournalEntryByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = "SELECT * FROM JournalEntry WHERE ID = @ID";
        return await connection.QueryFirstOrDefaultAsync<JournalEntry?>(query, new { ID = id });
    }

    public async Task<IEnumerable<JournalEntry>> GetJournalEntriesByGuardianIdAsync(Guid guardianId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = "SELECT * FROM JournalEntry WHERE GuardianID = @GuardianID";
        return await connection.QueryAsync<JournalEntry>(query, new { GuardianID = guardianId });
    }

    public async Task<IEnumerable<JournalEntry>> GetJournalEntriesByPatientIdAsync(Guid patientId)
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = "SELECT * FROM JournalEntry WHERE PatientID = @PatientID";
        return await connection.QueryAsync<JournalEntry>(query, new { PatientID = patientId });
    }

    public async Task AddJournalEntryAsync(JournalEntry journalEntry)
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"
                    INSERT INTO JournalEntry (ID, PatientID, GuardianID, Date, Title, Content, Rating)
                    VALUES (@ID, @PatientID, @GuardianID, @Date, @Title, @Content, @Rating)";
        await connection.ExecuteAsync(query, journalEntry);
    }

    public async Task UpdateJournalEntryAsync(JournalEntry journalEntry)
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = @"
                    UPDATE JournalEntry
                    SET PatientID = @PatientID, GuardianID = @GuardianID, Date = @Date, 
                        Title = @Title, Content = @Content , Rating = @Rating
                    WHERE ID = @ID";
        await connection.ExecuteAsync(query, journalEntry);
    }

    public async Task DeleteJournalEntryAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        const string query = "DELETE FROM JournalEntry WHERE ID = @ID";
        await connection.ExecuteAsync(query, new { ID = id });
    }
}