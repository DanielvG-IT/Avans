using Dapper;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace HQB.WebApi.Repositories;

public class GuardianRepository : IGuardianRepository
{
    private readonly string _connectionString;

    public GuardianRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Guardian>> GetAllGuardiansAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Guardian";
        return await connection.QueryAsync<Guardian>(sql);
    }

    public async Task<Guardian?> GetGuardianByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Guardian WHERE ID = @ID";
        return await connection.QueryFirstOrDefaultAsync<Guardian>(sql, new { ID = id });
    }

    public async Task<int> AddGuardianAsync(Guardian guardian)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "INSERT INTO Guardian (ID, FirstName, LastName, UserID) VALUES (@ID, @FirstName, @LastName, @UserID)";
        return await connection.ExecuteAsync(sql, guardian);
    }

    public async Task<int> UpdateGuardianAsync(Guardian guardian)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "UPDATE Guardian SET FirstName = @FirstName, LastName = @LastName, UserID = @UserID WHERE ID = @ID";
        return await connection.ExecuteAsync(sql, guardian);
    }

    public async Task<int> DeleteGuardianAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "DELETE FROM Guardian WHERE ID = @ID";
        return await connection.ExecuteAsync(sql, new { ID = id });
    }

    public async Task<Guardian?> GetGuardianByUserIdAsync(string userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = "SELECT * FROM Guardian WHERE UserID = @UserID";
        return await connection.QueryFirstOrDefaultAsync<Guardian>(sql, new { UserID = userId });
    }
}