using HQB.WebApi.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace HQB.WebApi.Services
{
  /// <summary>
  /// Based on the example code provided by Microsoft
  /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-9.0&preserve-view=true
  /// </summary>
  public class AspNetIdentityAuthenticationService : IAuthenticationService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<IdentityUser> _userManager;

    public AspNetIdentityAuthenticationService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
    {
      _httpContextAccessor = httpContextAccessor;
      _userManager = userManager;
    }

    /// <inheritdoc />
    public string? GetCurrentAuthenticatedUserId()
    {
      // Returns the aspnet_User.Id of the authenticated user
      return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<IList<string>> GetCurrentAuthenticatedUserRoles()
    {
      // Get the current authenticated user ID
      var userId = GetCurrentAuthenticatedUserId();
      if (userId == null)
      {
        return [];
      }

      // Get the user from the database using the UserManager
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        return [];
      }

      // Get the roles for the user
      return await _userManager.GetRolesAsync(user);
    }
  }
}