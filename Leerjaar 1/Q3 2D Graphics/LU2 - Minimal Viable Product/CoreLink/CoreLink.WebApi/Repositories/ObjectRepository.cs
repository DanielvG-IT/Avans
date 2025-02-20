using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CoreLink.WebApi.Repositories
{
    public class ObjectRepository(string sqlConnectionString) : IObjectRepository
    {

        // Create Methodes
        public async Task CreateObject(Guid EnvironmentId, Object2D object2d)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            var rowsAffected = await sqlConnection.ExecuteAsync("INSERT INTO [Object2D] (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)", new { object2d.Id, EnvironmentId, object2d.PrefabId, object2d.PositionX, object2d.PositionY, object2d.ScaleX, object2d.ScaleY, object2d.RotationZ, object2d.SortingLayer });
        }

        // public async Task CreateMultipleObjects(IEnumerable<Environment2D> listOfNewEnvironments)
        // {
        //     using (var sqlConnection = new SqlConnection(sqlConnectionString))
        //     {
        //         foreach (var environment in listOfNewEnvironments)
        //         {
        //             var rowsAffected = await sqlConnection.ExecuteAsync("INSERT INTO [Object2D] (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)", environment);

        //             // TODO ASK MARC IF THIS IS THE RIGHT WAY TO HANDLE THIS
        //             // if (rowsAffected == 0) return false;
        //             // else return true;
        //         }

        //     }
        // }


        // Read Methodes
        // public async Task<IEnumerable<Object2D>> GetObjectByID(Guid EnvironmentId)
        // {
        //     using (var sqlConnection = new SqlConnection(sqlConnectionString))
        //     {
        //         return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE  ", new { EnvironmentId });
        //     }
        // }

        public Task<Object2D?> GetObjectById(Guid ObjectId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return sqlConnection.QuerySingleOrDefaultAsync<Object2D>("SELECT * FROM [Object2D] WHERE Id = @ObjectId", new { ObjectId });
        }

        public async Task<IEnumerable<Object2D>> GetObjectsByEnvironmentId(Guid EnvironmentId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE EnvironmentId = @EnvironmentId", new { EnvironmentId });
        }


        // Update Methodes
        public async Task UpdateObjectById(Guid Id, Object2D object2D)
        {
            // TODO Check what to update
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync("UPDATE [Object2D] SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer WHERE Id = @Id", new { object2D.PrefabId, object2D.PositionX, object2D.PositionY, object2D.ScaleX, object2D.ScaleY, object2D.RotationZ, object2D.SortingLayer, Id });
        }


        // Delete Methodes
        public async Task DeleteObjectById(Guid id)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync("DELETE FROM [Object2D] WHERE Id = @Id", new { id });
        }
    }
}