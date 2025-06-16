namespace Api.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that imports trash data.
    /// </summary>
    public interface ITrashImportService
    {
        /// <summary>
        /// Asynchronously imports trash data.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to cancel the import operation.</param>
        /// <returns>
        /// A task that represents the asynchronous import operation.
        /// The task result is <c>true</c> if the import was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ImportAsync(CancellationToken ct);
    }
}