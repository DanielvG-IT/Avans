using CoreLink.WebApi.Models;
using CoreLink.WebApi.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;

namespace CoreLink.WebApi.Repositories;

public class ObjectRepository : IObjectRepository
{
    private readonly string sqlConnectionString;

    public ObjectRepository(string sqlDatabaseConnectionString)
    {
        sqlConnectionString = sqlDatabaseConnectionString;
    }

    // TODO Refactor SQL queries and parameters
    public async Task<IEnumerable<Object2D>> GetObjectsAsync(Guid environmentId)
    {
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D] WHERE environmentId = @environmentId", new { environmentId });
    }

    public async Task<Object2D?> GetObjectByIdAsync(Guid objectId)
    {
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        return await sqlConnection.QuerySingleOrDefaultAsync<Object2D>("SELECT * FROM [Object2D] WHERE id = @objectId", new { objectId });
    }

    public async Task CreateObject(Guid environmentId, Object2D newObject)
    {
        // TODO Refactor methode in controller to set environmentId in the object
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        await sqlConnection.ExecuteAsync("INSERT INTO [Object2D] (id, environmentId, prefabId, positionX, positionY, scaleX, scaleY, rotationZ, sortingLayer) VALUES (@id, @environmentId, @prefabId, @positionX, @positionY, @scaleX, @scaleY, @rotationZ, @sortingLayer)", new { newObject.id, environmentId, newObject.prefabId, newObject.positionX, newObject.positionY, newObject.scaleX, newObject.scaleY, newObject.rotationZ, newObject.sortingLayer });
    }

    public async Task UpdateObjectByIdAsync(Guid id, Object2D updatedObject)
    {
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        await sqlConnection.ExecuteAsync("UPDATE [Object2D] SET prefabId = @prefabId, positionX = @positionX, positionY = @positionY, scaleX = @scaleX, scaleY = @scaleY, rotationZ = @rotationZ, sortingLayer = @sortingLayer WHERE id = @id", new { updatedObject.environmentId, updatedObject.prefabId, updatedObject.positionX, updatedObject.positionY, updatedObject.scaleX, updatedObject.scaleY, updatedObject.rotationZ, updatedObject.sortingLayer, id });
    }

    public async Task DeleteObjectByIdAsync(Guid id)
    {
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        await sqlConnection.ExecuteAsync("DELETE FROM [Object2D] WHERE id = @id", new { id });
    }

    public async Task DeleteObjectsByEnvironmentIdAsync(Guid envId)
    {
        using var sqlConnection = new SqlConnection(sqlConnectionString);
        await sqlConnection.ExecuteAsync("DELETE FROM [Object2D] WHERE environmentId = @envId", new { envId });

    }
}