using System.Text;
using HQB.WebApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using HQB.WebApi.Data;
using Microsoft.Extensions.Options;

namespace HQB.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<OuderVoogd> _signInManager;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly UserManager<OuderVoogd> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtOptions _jwtOptions;

        public AuthController(
            UserManager<OuderVoogd> userManager,
            SignInManager<OuderVoogd> signInManager,
            ILogger<AuthController> logger,
            IOptions<JwtOptions> jwtOptions,
            JwtSecurityTokenHandler tokenHandler)
        {
            _signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            _tokenHandler = tokenHandler;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The registration model.</param>
        /// <returns>An IActionResult indicating the result of the registration.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { code = "InvalidData", message = "Invalid data.", details = modelErrors });
            }

            var user = new OuderVoogd
            {
                UserName = model.Email,
                Email = model.Email,
                Voornaam = model.Voornaam,
                Achternaam = model.Achternaam
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} created successfully.", model.Email);
                return Ok(new { message = "User created successfully" });
            }

            var errors = result.Errors.Select(e => new
            {
                code = e.Code,
                message = e.Description
            });
            _logger.LogWarning("User creation failed for {Email}: {Errors}", model.Email, errors);
            return BadRequest(new { errors });
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>An IActionResult containing the JWT token if successful.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { code = "InvalidData", message = "Invalid data.", details = errors });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed for {Email}: User not found.", model.Email);
                return Unauthorized(new { code = "InvalidCredentials", message = "Invalid credentials." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for {Email}: Invalid password.", model.Email);
                return Unauthorized(new { code = "InvalidCredentials", message = "Invalid credentials." });
            }

            string tokenString = GenerateJwtToken(user);
            _logger.LogInformation("User {Email} logged in successfully.", model.Email);
            return Ok(new { token = new { accessToken = tokenString } });
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The generated JWT token.</returns>
        private string GenerateJwtToken(OuderVoogd user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwtOptions.ExpireDays),
                signingCredentials: creds
            );

            return _tokenHandler.WriteToken(token);
        }
    }

    public static class ResultExtensions
    {
        public static IActionResult ToBadRequest(this IdentityResult result)
        {
            var errors = result.Errors.Select(e => new { e.Code, e.Description });
            return new BadRequestObjectResult(new { errors });
        }
    }

    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Voornaam { get; set; }
        public required string Achternaam { get; set; }
    }

    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}