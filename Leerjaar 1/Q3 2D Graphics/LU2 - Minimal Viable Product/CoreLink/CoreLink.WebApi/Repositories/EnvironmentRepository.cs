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
            var rowsAffected = await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (id, name, ownerUserId, maxHeight, maxLength) VALUES (@Id, @Name, @ownerUserId ,@MaxHeight, @MaxLength)", environment);
        }

        public async Task<Environment2D?> GetEnvironmentById(Guid Id)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE id = @Id", new { Id });
        }

        public async Task<IEnumerable<Environment2D>> GetEnvironmentsByUserId(string UserId)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            var userEnvironments = await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE ownerUserId = @UserId", new { UserId });
            return userEnvironments;
        }

        public async Task UpdateEnvironmentById(Guid id, Environment2D updatedEnvironment)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            var query = "UPDATE [Environment2D] SET " +
                "name = @Name, " +
                "maxHeight = @MaxHeight, " +
                "maxLength = @MaxLength " +
                "WHERE id = @Id";
            var parameters = new
            {
                updatedEnvironment.name,
                updatedEnvironment.maxHeight,
                updatedEnvironment.maxLength,
                Id = id
            };
            await sqlConnection.ExecuteAsync(query, parameters);
        }

        public async Task DeleteEnvironmentById(Guid id)
        {
            using var sqlConnection = new SqlConnection(sqlDatabaseConnectionString);
            await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
        }
    }
}
