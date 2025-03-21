using System.Text;
using HQB.WebApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace HQB.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<OuderVoogd> _userManager;
        private readonly SignInManager<OuderVoogd> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<OuderVoogd> userManager, SignInManager<OuderVoogd> signInManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
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
                Voornaam = model.FirstName,
                Achternaam = model.LastName
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { code = "InvalidData", message = "Invalid data.", details = modelErrors });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed for {Email}: User not found.", model.Email);
                return Unauthorized(new { code = "InvalidCredentials", message = "Invalid credentials." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for {Email}: Invalid credentials.", model.Email);
                return Unauthorized(new { code = "InvalidCredentials", message = "Invalid credentials." });
            }

            // Generate JWT Token
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
            };

            var jwtConfig = new
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Key = _configuration["Jwt:Key"],
                ExpireDays = _configuration["Jwt:ExpireDays"]
            };

            if (string.IsNullOrEmpty(jwtConfig.Key) || jwtConfig.Key.Length < 32)
            {
                _logger.LogError("JWT key is not configured properly or is too short.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { code = "ServerError", message = "JWT key is not configured properly or is too short." });
            }

            if (jwtConfig.ExpireDays == null)
            {
                _logger.LogError("JWT expiration days configuration is missing.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { code = "ServerError", message = "JWT expiration days configuration is missing." });
            }
            if (!int.TryParse(jwtConfig.ExpireDays.ToString(), out var expireDays))
            {
                _logger.LogError("JWT expiration days configuration is invalid.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { code = "ServerError", message = "JWT expiration days configuration is invalid." });
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(expireDays),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("User {Email} logged in successfully.", model.Email);
            return Ok(new { token = new { accessToken = tokenString } });
        }
    }
}