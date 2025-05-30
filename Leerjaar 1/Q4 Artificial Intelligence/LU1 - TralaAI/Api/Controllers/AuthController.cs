using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TralaAI.CoreApi.Models.Auth;
using TralaAI.CoreApi.Interfaces;

namespace TralaAI.CoreApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJwtService jwtService) : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IJwtService _jwtService = jwtService;

        [HttpPost()]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false); // @DanielvG-IT Don't lock user out by using false
            if (!result.Succeeded)
                return Unauthorized();

            var expiresAt = DateTime.Now.AddMinutes(60);
            var token = _jwtService.GenerateJwtToken(user, expiresAt);

            return Ok(new
            {
                token,
                expiresAt
            });
        }

        // TODO: May add RefreshToken logic for better experience
    }
}