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

        [HttpGet("roles", Name = "GetUserRoles")]
        public async Task<ActionResult<IList<string>>> GetRolesForUser()
        {
            var roles = await _authenticationService.GetCurrentAuthenticatedUserRoles();
            return Ok(roles);
        }
    }
}
