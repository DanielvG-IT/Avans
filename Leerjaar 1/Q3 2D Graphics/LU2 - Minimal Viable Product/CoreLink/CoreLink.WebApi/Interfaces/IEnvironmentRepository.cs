using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Interfaces
{
    /// <summary>
    /// Interface for managing 2D environments.
    /// </summary>
    public interface IEnvironmentRepository
    {
        /// <summary>
        /// Creates a new 2D environment.
        /// </summary>
        /// <param name="environment">The environment to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task CreateEnvironment(Environment2D environment);

        /// <summary>
        /// Retrieves all 2D environments.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of environments.</returns>
        public Task<IEnumerable<Environment2D>> GetAllEnvironments();

        /// <summary>
        /// Retrieves a 2D environment by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the environment.</param>
        /// <returns>A task representing the asynchronous operation, containing the environment if found, otherwise null.</returns>
        public Task<Environment2D?> GetEnvironmentById(Guid Id);

        /// <summary>
        /// Retrieves a 2D environment by the user's unique identifier.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the environment if found, otherwise null.</returns>
        public Task<IEnumerable<Environment2D>> GetEnvironmentByUserId(Guid UserId);

        /// <summary>
        /// Updates a 2D environment by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the environment to update.</param>
        /// <param name="updatedEnvironment">The updated environment data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdateEnvironmentById(Guid Id, Environment2D updatedEnvironment);

        /// <summary>
        /// Deletes a 2D environment by its unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the environment to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteEnvironmentById(Guid Id);
    }
}
