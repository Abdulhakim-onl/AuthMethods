using JWTPermissionBased.Application.Common.Configs;
using JWTPermissionBased.Application.Common.Enums.AuthEnums;
using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Application.Common.Models;
using JWTPermissionBased.Domain.AuthEntity;
using JWTPermissionBased.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;

namespace JWTPermissionBased.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationContext _context;
    private readonly GoogleExternalOptions _googleExternal;
    private readonly IGoogleAuthenticatorService _googleAuthenticatorService;

    public AuthenticationController(IAuthService authService, ApplicationContext context, GoogleExternalOptions googleExternal, IGoogleAuthenticatorService googleAuthenticatorService)
    {
        _authService = authService;
        _googleExternal = googleExternal;
        _context = context;
        _googleAuthenticatorService = googleAuthenticatorService;
    }

    [AllowAnonymous]
    [HttpPost("registration_with_email")]
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

        await _context.Users.AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(await _authService.AuthenticationToken(user));
    }

    [AllowAnonymous]
    [HttpPost("login_with_email")]
    public async Task<IActionResult> LoginWithEmail([FromBody] LoginViaEmailCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            throw new ArgumentNullException(typeof(LoginViaEmailCommand).ToString(), "Not valid");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email && x.Password == command.Password, cancellationToken) ?? throw new Exception("User doesn't exist");

        return Ok(await _authService.AuthenticationToken(user));
    }

    [AllowAnonymous]
    [HttpPost("login_with_google")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] ExternalAuthModel authModel, CancellationToken cancellationToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { _googleExternal.ClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(authModel.IdToken, settings);

        var user = _context.Users.Where(x => x.ExternalId == authModel.Id && x.Provider == authModel.Provider).FirstOrDefault();

        if (user == null)
        {
            user = new User()
            {
                Id = Guid.NewGuid(),
                UserName = authModel.Name,
                Email = authModel.Email,
                FirstName = authModel.FirstName,
                LastName = authModel.LastName,
                ExternalId = authModel.Id,
                Provider = authModel.Provider,
                PhotoUrl = authModel.PhotoUrl,
                RoleId = RoleEnum.Client.Key
            };

            await _context.Users.AddAsync(user, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        if (user != null)
        {
            return Ok(await _authService.AuthenticationToken(user));
        }
        else
        {
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpGet("reg_authenticator")]
    public IActionResult RegVerification()
    {
        var SetupResult = _googleAuthenticatorService.GenerateSetupCode("Test2Authenticator", "secretkey", 250, 250);
        return Ok(SetupResult.QrCodeSetupImageUrl + ' ' + SetupResult.ManualEntryKey);
    }

    [AllowAnonymous]
    [HttpPost("auth_authenticator")]
    public IActionResult ValidateAuthenticator([FromBody] string secretCode)
    {
        bool ValidateResult = _googleAuthenticatorService.ValidateTwoFactorPIN("secretkey", secretCode);

        return Ok(ValidateResult);
    }
}
