using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialAuth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfigurationSection _jwtSettings;

        public AuthController(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            _jwtSettings = configuration.GetSection("JwtSettings");

        }

        [Authorize]
        [HttpGet("Test")]
        public string Test()
        {
            return "accounts controller";
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationModel userModel)
        {
            var user = new User() { UserName = userModel.UserName, Email = userModel.Email, FirstName = userModel.FirstName, LastName = userModel.LastName };
            var result = await userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                return Ok(result.Errors);
            }
            await userManager.AddToRoleAsync(user, "Visitor");

            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userModel)
        {
            var user = await userManager.FindByEmailAsync(userModel.Email);

            if (user != null && await userManager.CheckPasswordAsync(user, userModel.Password))
            {
                var claims = GetClaims(user);
                return Ok(GenerateTokenOptions(await claims));
            }
            return Unauthorized("Invalid Authentication");
        }

        private string GenerateTokenOptions(List<Claim> claims)
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
            var secret = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings.GetSection("validIssuer").Value,
                audience: _jwtSettings.GetSection("validAudience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
