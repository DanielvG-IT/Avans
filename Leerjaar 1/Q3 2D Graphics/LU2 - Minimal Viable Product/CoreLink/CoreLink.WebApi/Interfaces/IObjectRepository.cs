using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Interfaces
{
    /// <summary>
    /// Interface for managing 2D objects in a repository.
    /// </summary>
    public interface IObjectRepository
    {
        /// <summary>
        /// Retrieves a collection of 2D objects associated with a specific environment.
        /// </summary>
        /// <param name="environmentId">The unique identifier of the environment.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 2D objects.</returns>
        Task<IEnumerable<Object2D>> GetObjectsAsync(Guid environmentId);

        /// <summary>
        /// Retrieves a 2D object by its unique identifier.
        /// </summary>
        /// <param name="objectId">The unique identifier of the 2D object.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the 2D object if found; otherwise, null.</returns>
        Task<Object2D?> GetObjectByIdAsync(Guid objectId);

        /// <summary>
        /// Creates a new 2D object in the specified environment.
        /// </summary>
        /// <param name="environmentId">The unique identifier of the environment.</param>
        /// <param name="newObject">The new 2D object to be created.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateObject(Guid environmentId, Object2D newObject);

        /// <summary>
        /// Updates an existing 2D object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the 2D object to be updated.</param>
        /// <param name="updatedObject">The updated 2D object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateObjectByIdAsync(Guid id, Object2D updatedObject);

        /// <summary>
        /// Deletes a 2D object by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the 2D object to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteObjectByIdAsync(Guid id);

        /// <summary>
        /// Deletes all 2D objects associated with a specific environment.
        /// </summary>
        /// <param name="environmentId">The unique identifier of the environment.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteObjectsByEnvironmentIdAsync(Guid environmentId);
    }
}
