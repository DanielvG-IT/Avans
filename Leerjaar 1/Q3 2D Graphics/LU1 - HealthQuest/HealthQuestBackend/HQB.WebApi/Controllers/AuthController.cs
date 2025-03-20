using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HQB.WebApi.Models;

namespace HQB.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // This allows non-logged-in users to access the endpoints in this controller
    public class AuthController(UserManager<OuderVoogd> userManager, SignInManager<OuderVoogd> signInManager) : ControllerBase
    {
        private readonly UserManager<OuderVoogd> _userManager = userManager;
        private readonly SignInManager<OuderVoogd> _signInManager = signInManager;

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new OuderVoogd
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Voornaam = model.FirstName,
                    Achternaam = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new { message = "User created successfully" });
                }
                else
                {
                    var errors = result.Errors.Select(e => new
                    {
                        code = e.Code,
                        message = e.Description
                    });
                    return BadRequest(new { errors });
                }
            }
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { code = "InvalidData", message = "Invalid data.", details = modelErrors });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                        return Ok(new { message = "Login successful" });

                    else if (result.IsLockedOut)
                        return Unauthorized(new { code = "UserLockedOut", message = "User account is locked out." });

                    else if (result.IsNotAllowed)
                        return Unauthorized(new { code = "NotAllowed", message = "User is not allowed to sign in." });

                    else if (result.RequiresTwoFactor)
                        return Unauthorized(new { code = "TwoFactorRequired", message = "Two-factor authentication is required." });

                    else
                        return Unauthorized(new { code = "InvalidCredentials", message = "Invalid credentials." });
                }
                else
                    return Unauthorized(new { code = "InvalidCredentials", message = "Invalid credentials." });
            }
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { code = "InvalidData", message = "Invalid data.", details = modelErrors });
        }
    }
}