using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using HomeCareApp.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppUser = HomeCareApp.Models.User;

namespace HomeCareApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
[AllowAnonymous]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var user = new AppUser
    {
        // Vi bruker e-post som brukernavn i Identity
        UserName = registerDto.Email,
        Email = registerDto.Email,
        FullName = registerDto.FullName
    };

    var result = await _userManager.CreateAsync(user, registerDto.Password);

    if (!result.Succeeded)
    {
        _logger.LogWarning("[AuthController] Registration failed for {Email}. Errors: {@Errors}", registerDto.Email, result.Errors);
        return BadRequest(result.Errors);
    }

    // gi standarrolle "Patient" til nyregistrert bruker
    var roleResult = await _userManager.AddToRoleAsync(user, "Patient");
    if (!roleResult.Succeeded)
    {
        _logger.LogError("[AuthController] Failed to add role 'Patient' for {Email}. Errors: {@Errors}", registerDto.Email, roleResult.Errors);
       
        return StatusCode(500, new { message = "User created but failed to assign Patient role." });
    }

    _logger.LogInformation("[AuthController] User registered with Patient role: {Email}", registerDto.Email);
    return Ok(new { message = "User registered successfully" });
}


        // POST: api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Vi logger inn med e-post
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogInformation("[AuthController] User authorised: {Email}", loginDto.Email);

                var token = await GenerateJwtToken(user);
                return Ok(new { token });
            }

            _logger.LogWarning("[AuthController] User not authorised: {Email}", loginDto.Email);
            return Unauthorized("Invalid email or password");
        }

        // POST: api/Auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("[AuthController] User logged out");
            return Ok(new { message = "Logout successful" });
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError("[AuthController] JWT key is missing from configuration.");
                throw new InvalidOperationException("JWT key is missing from configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // Bruk e-post som "sub"
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

                // Identity-bruker-ID
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                // Fullt navn (ClaimTypes.Name → blir "name"-claim i JWT)
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64
                )
            };

            // Roller
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
