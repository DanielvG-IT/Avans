/// <summary>
/// Defines the contract for a service that handles API key validation.
/// </summary>
namespace Api.Interfaces
{
    public interface IApiKeyService
    {
        /// <summary>
        /// Checks if the provided API key is valid.
        /// </summary>
        /// <param name="apiKey">The API key to validate.</param>
        /// <returns><c>true</c> if the API key is valid; otherwise, <c>false</c>.</returns>
        bool IsValidApiKey(Guid apiKey);
    }
}