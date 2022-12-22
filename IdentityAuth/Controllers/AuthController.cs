using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using IdentityAuth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.ViewModels;
using IdentityAuth.Interfaces;

namespace IdentityAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthController(UserManager<User> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
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
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                return Ok(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Visitor");

            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, userModel.Password))
            {
                var claims = await _authService.GetClaims(user);
                return Ok(await _authService.GenerateTokenOptions(claims));
            }
            return Unauthorized("Invalid Authentication");
        }
    }
}
