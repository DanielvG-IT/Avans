namespace CoreLink.WebApi.Interfaces
{
  /// <summary>
  /// Provides methods for authentication-related operations.
  /// </summary>
  public interface IAuthenticationService
  {
    /// <summary>
    /// Retrieves the ID of the currently authenticated user using the Identity framework.
    /// </summary>
    /// <returns>A string representing the ID of the authenticated user, or null if no user is authenticated.</returns>
    public string? GetCurrentAuthenticatedUserId();
  }
}