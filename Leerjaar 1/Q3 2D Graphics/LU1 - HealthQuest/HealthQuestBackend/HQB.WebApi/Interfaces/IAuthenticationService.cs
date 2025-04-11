namespace HQB.WebApi.Interfaces;
/// <summary>
/// Provides methods for authentication-related operations.
/// </summary>
public interface IAuthenticationService
{
  /// <summary>
  /// Retrieves the ID of the currently authenticated user using the Identity framework.
  /// </summary>
  /// <returns>A string representing the ID of the authenticated user, or null if no user is authenticated.</returns>
  /// <remarks>
  /// This method uses the IHttpContextAccessor to access the current HTTP context and retrieve the user ID from the claims.
  /// If the user is not authenticated, null is returned.
  /// </remarks>
  /// <example>
  /// var userId = _authenticationService.GetCurrentAuthenticatedUserId();
  /// if (userId != null)
  /// {
  ///     // User is authenticated, do something with the user ID
  /// }
  /// else
  /// {
  ///     // User is not authenticated
  /// }
  /// </example>
  public string? GetCurrentAuthenticatedUserId();

  /// <summary>
  /// Retrieves the roles of the currently authenticated user using the Identity framework.
  /// </summary>
  /// <returns>A list of strings representing the roles of the authenticated user.</returns>
  /// <remarks>
  /// This method uses the UserManager to retrieve the roles of the user.
  /// If the user is not authenticated or does not exist, an empty list is returned.
  /// </remarks>
  /// <example>
  /// var roles = await _authenticationService.GetCurrentAuthenticatedUserRoles();
  /// if (roles.Count > 0)
  /// {
  ///    // User has roles, do something with the roles
  /// }
  /// else
  /// {
  ///    // User has no roles or is not authenticated
  /// }
  /// </example>
  public Task<IList<string>> GetCurrentAuthenticatedUserRoles();
}