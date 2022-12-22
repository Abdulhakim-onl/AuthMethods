using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using JWTPermissionBased.Application.Common.Constants;
using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JWTPermissionBased.Application.Common.Services;

public class CurrentUserService : ICurrentUserService
{
    private Guid _userId = Guid.Empty;
    private Guid _roleId= Guid.Empty;

    public Guid? UserId => _userId;
    public Guid? RoleId => _roleId;
    public List<string> Permissions { get; set; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext != null) GetData(httpContextAccessor.HttpContext);
    }

    private async void GetData(HttpContext context)
    {
        var request = context.Request.HttpContext.Request;
        var header = request.Headers["LuckySecurity"].ToString();
        if ( string.IsNullOrWhiteSpace(header))
        {
            return;
        }

        var tokenMatch = Regex.Match(header, AuthSchemeConstants.LToken);

        if (!tokenMatch.Success)
        {
            throw new Exception("Token invalid");
        }

        var token = tokenMatch.Groups["token"].Value;
        
        var handler = new JwtSecurityTokenHandler();
        var readJwt = handler.ReadJwtToken(token);
        _userId= Guid.Parse(readJwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value) ;
        _roleId= Guid.Parse(readJwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value);
    }
}