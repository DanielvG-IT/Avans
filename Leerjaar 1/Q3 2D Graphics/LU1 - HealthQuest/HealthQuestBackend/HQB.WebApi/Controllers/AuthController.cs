using HQB.WebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<OuderVoogd> userManager, SignInManager<OuderVoogd> signInManager) : ControllerBase
    {
        private readonly UserManager<OuderVoogd> _userManager = userManager;
        private readonly SignInManager<OuderVoogd> _signInManager = signInManager;

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new OuderVoogd
                {
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
                    var errors = result.Errors.Select(e => new Error
                    {
                        code = e.Code,
                        message = e.Description,
                        details = string.Empty
                    });
                    return BadRequest(errors);
                }
            }
            return BadRequest(new Error { code = "InvalidData", message = "Invalid data.", details = string.Empty });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        return Ok(new { message = "Login successful" });
                    }
                    return Unauthorized(new Error { code = "InvalidCredentials", message = "Invalid credentials.", details = string.Empty });
                }
                return Unauthorized(new Error { code = "InvalidCredentials", message = "Invalid credentials.", details = string.Empty });
            }
            return BadRequest(new Error { code = "InvalidData", message = "Invalid data.", details = string.Empty });
        }
    }
}
