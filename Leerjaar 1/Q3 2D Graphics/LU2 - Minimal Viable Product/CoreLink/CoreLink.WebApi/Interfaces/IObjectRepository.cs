using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Interfaces
{
    /// <summary>
    /// Interface for managing 2D objects within different environments.
    /// </summary>
    public interface IObjectRepository
    {
        /// <summary>
        /// Creates a new 2D object in the specified environment.
        /// </summary>
        /// <param name="EnvironmentId">The unique identifier of the environment.</param>
        /// <param name="object2d">The 2D object to be created.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task CreateObject(Guid EnvironmentId, Object2D object2d);

        /// <summary>
        /// Retrieves all 2D objects within the specified environment.
        /// </summary>
        /// <param name="EnvironmentId">The unique identifier of the environment.</param>
        /// <returns>A task representing the asynchronous operation, with a result of an enumerable collection of 2D objects.</returns>
        public Task<Object2D?> GetObjectById(Guid ObjectId);

        /// <summary>
        /// Retrieves all 2D objects within the specified environment.
        /// </summary>
        /// <param name="EnvironmentId">The unique identifier of the environment.</param>
        /// <returns>A task representing the asynchronous operation, with a result of an enumerable collection of 2D objects.</returns>
        public Task<IEnumerable<Object2D>> GetObjectsByEnvironmentId(Guid EnvironmentId);

        /// <summary>
        /// Updates an existing 2D object by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the 2D object to be updated.</param>
        /// <param name="object2D">The updated 2D object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdateObjectById(Guid Id, Object2D object2D);

        /// <summary>
        /// Deletes a 2D object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the 2D object to be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteObjectById(Guid id);
    }
}
