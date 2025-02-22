using CoreLink.WebApi.Models;
using CoreLink.WebApi.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CoreLink.WebApi.Repositories
{
    public class EnvironmentRepository(string sqlDatabaseConnectionString) : IEnvironmentRepository
    {
        public async Task CreateEnvironmentAsync(Environment2D environment)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            var rowsAffected = await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (id, name, ownerUserId, maxHeight, maxLength) VALUES (@id, @name, @ownerUserId ,@maxHeight, @maxLength)", environment);
        }

        public async Task<Environment2D?> GetEnvironmentByIdAsync(Guid Id)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE id = @id", new { Id });
        }

        public async Task<IEnumerable<Environment2D>> GetEnvironmentsByUserIdAsync(string UserId)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            var userEnvironments = await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE ownerUserId = @UserId", new { UserId });
            return userEnvironments;
        }

        public async Task UpdateEnvironmentByIdAsync(Guid id, Environment2D updatedEnvironment)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (id, name, ownerUserId, maxHeight, maxLength) VALUES (@id, @name, @ownerUserId, @maxHeight, @maxLength)", updatedEnvironment);
        }

        public async Task DeleteEnvironmentByIdAsync(Guid id)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
        }
    }
}
