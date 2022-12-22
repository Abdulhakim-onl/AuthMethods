using Google.Apis.Auth;
using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Application.Common.Models;
using JWTPermissionBased.Domain.AuthEntity;
using JWTPermissionBased.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace JWTPermissionBased.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationContext _context;

    public AuthenticationController(IAuthService authService, ApplicationContext context)
    {
        _authService = authService;
        _context = context;
    }
    
    [AllowAnonymous]
    [HttpPost("registration_with email")]
    public async Task<IActionResult> RegistrationWithEmail([FromBody] RegistrationViaEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            throw new ArgumentNullException(typeof(RegistrationViaEmailCommand).ToString(), "Not valid");


        if (await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken) != null)
            throw new ArgumentNullException(nameof(command), $"User with email {command.Email} exist");

        var user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = Guid.NewGuid().ToString(),
            Email = command.Email,
            RoleId = RoleEnum.Client.Key,
            Password = command.Password
        };
        
        await _context.Users.AddAsync(user,cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(await _authService.AuthenticationToken(user));
    }
    
    [AllowAnonymous]
    [HttpPost("login_with email")]
    public async Task<IActionResult> LoginWithEmail([FromBody] LoginViaEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            throw new ArgumentNullException(typeof(LoginViaEmailCommand).ToString(), "Not valid");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email && x.Password == command.Password, cancellationToken) ?? throw new Exception("User doesn't exist");
      
        return Ok(await _authService.AuthenticationToken(user));
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticateRequest data, CancellationToken cancellationToken)
    {
        GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

        // Change this to your google client ID
        settings.Audience = new List<string>() { "576415067343-27vc72nhm2mq28iepvpsn4ell0reg0ln.apps.googleusercontent.com" };

        GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == payload.Email, cancellationToken) 
            ?? throw new Exception("User doesn't exist");

        return Ok(new { AuthToken = _authService.AuthenticationToken(user) });
    }

    public class AuthenticateRequest
    {
        [Required]
        public string IdToken { get; set; }
    }
}
