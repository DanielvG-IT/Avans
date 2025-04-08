using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HQB.WebApi.Controllers
{

    [Route("api/v1/auth")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthenticationService _authenticationService;

        public RoleController(UserManager<IdentityUser> userManager, IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _authenticationService = authenticationService;
        }

        [HttpGet("/roles", Name = "GetUserRoles")]
        public async Task<ActionResult<IList<string>>> GetRolesForUser()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                return NotFound();
            }

            return Ok(roles);
        }
    }
}
