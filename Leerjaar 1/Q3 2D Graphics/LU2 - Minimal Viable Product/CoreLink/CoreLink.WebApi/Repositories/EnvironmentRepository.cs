using CoreLink.WebApi.Models;
using CoreLink.WebApi.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CoreLink.WebApi.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly string sqlConnectionString;

        public EnvironmentRepository(string sqlDatabaseConnectionString)
        {
            sqlConnectionString = sqlDatabaseConnectionString;
        }

        // Create Methodes
        public async Task CreateAsync(Environment2D environment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                var environmentId = await sqlConnection.ExecuteAsync("INSERT INTO [Environment2D] (Id, Name, MaxHeight, MaxLength) VALUES (@Id, @Name, @MaxHeight, @MaxLength)", environment);
            }
        }


        // Read Methodes
        public async Task<IEnumerable<Environment2D>> ReadAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Environment2D>("SELECT * FROM [Environment2D]");
            }
        }

        public async Task<Environment2D?> ReadAsync(Guid Id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT * FROM [Environment2D] WHERE Id = @Id", new { Id });
            }
        }


        // Update Methodes
        public async Task UpdateAsync(Environment2D environment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                                 "Id = @Id, " +
                                                 "Name = @Name" +
                                                 "MaxHeight = @MaxHeight" +
                                                 "MaxLength = @MaxLength"
                                                 , environment);

            }
        }


        // Delete Methodes
        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [WeatherForecast] WHERE Id = @Id", new { id });
            }
        }
    }
}
