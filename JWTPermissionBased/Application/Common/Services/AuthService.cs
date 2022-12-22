using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JWTPermissionBased.Application.Common.Configs;
using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Application.Common.Models;
using JWTPermissionBased.Domain.AuthEntity;
using JWTPermissionBased.Infrastructure;

namespace JWTPermissionBased.Application.Common.Services;

public class AuthService: IAuthService
{
    private readonly JwtTokenConfig _config;
    private readonly ApplicationContext _context;

    public AuthService(JwtTokenConfig config, ApplicationContext context)
    {
        _config = config;
        _context = context;
    }
    
    public async Task<JwtAuthResultDto> AuthenticationToken(User userAuth)
    {
        return await Task.FromResult(new JwtAuthResultDto()
        {
            AccessToken = await AccessGenerator(userAuth),
            RefreshToken = await RefreshGenerator(userAuth.Id)
        });
    }

    private async Task<string> AccessGenerator(User userAuth)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Sid, userAuth.Id.ToString()),
            new Claim(ClaimTypes.Role,  userAuth.RoleId.ToString())
        };

        return await GenerateToken(claims, _config.AccessTokenExpiration);
    }

    private async Task<string> RefreshGenerator(Guid userId)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Sid, userId.ToString())
        };

        return await GenerateToken(claims, _config.RefreshTokenExpiration);
    }

    private Task<string> GenerateToken(IEnumerable<Claim> claims, int expires)
    {
        JwtSecurityToken securityToken = new(
            _config.Issuer,
            _config.Audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(expires),
            _config.GetSigningCredentials(_config.GetSymmetricSecurityKey()));

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(securityToken));
    }
}