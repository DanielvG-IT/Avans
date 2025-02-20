using CoreLink.WebApi.Models;
using CoreLink.WebApi.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CoreLink.WebApi.Repositories
{
    public class EnvironmentRepository(string sqlDatabaseConnectionString) : IEnvironmentRepository
    {
        public async Task CreateEnvironment(Environment2D environment)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            var rowsAffected = await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, MaxHeight, MaxLength) VALUES (@Id, @Name, @MaxHeight, @MaxLength)", environment);
        }

        public async Task<IEnumerable<Environment2D>> GetAllEnvironments()
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");
        }

        public async Task<Environment2D?> GetEnvironmentById(Guid Id)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { Id });
        }

        public async Task<IEnumerable<Environment2D>> GetEnvironmentByUserId(Guid UserId)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            var userEnvironments = await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE UserId = @UserId", new { UserId });
            return userEnvironments;
        }

        public async Task UpdateEnvironmentById(Guid Id, Environment2D updatedEnvironment)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                             "Name = @Name, " +
                                             "MaxHeight = @MaxHeight, " +
                                             "MaxLength = @MaxLength " +
                                             "WHERE Id = @Id",
                                             new { updatedEnvironment.Name, updatedEnvironment.MaxHeight, updatedEnvironment.MaxLength, Id });
        }

        public async Task DeleteEnvironmentById(Guid id)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
        }
    }
}
