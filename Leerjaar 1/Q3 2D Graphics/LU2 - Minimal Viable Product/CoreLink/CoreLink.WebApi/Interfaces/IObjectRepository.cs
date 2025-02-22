using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Interfaces
{
    /// <summary>
    /// Interface for managing 2D objects in a repository.
    /// </summary>
    public interface IObjectRepository
    {
        /// <summary>
        /// Retrieves a collection of 2D objects asynchronously based on the specified environment ID.
        /// </summary>
        /// <param name="environmentId">The ID of the environment to retrieve objects from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 2D objects.</returns>
        Task<IEnumerable<Object2D>> GetObject2DsAsync(Guid environmentId);

        Task<Object2D?> GetObjectByIdAsync(Guid objectId);

        /// <summary>
        /// Creates a new 2D object in the specified environment asynchronously.
        /// </summary>
        /// <param name="environmentId">The ID of the environment to create the object in.</param>
        /// <param name="newObject">The new 2D object to create.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CreateObject(Guid environmentId, Object2D newObject);

        /// <summary>
        /// Updates an existing 2D object asynchronously.
        /// </summary>
        /// <param name="id">The ID of the object to update.</param>
        /// <param name="updatedObject">The updated 2D object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateObject(Guid id, Object2D updatedObject);

        /// <summary>
        /// Deletes an existing 2D object asynchronously.
        /// </summary>
        /// <param name="id">The ID of the object to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteObject(Guid id);
    }
}
