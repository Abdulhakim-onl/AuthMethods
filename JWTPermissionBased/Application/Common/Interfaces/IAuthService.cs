using JWTPermissionBased.Application.Common.Models;
using JWTPermissionBased.Domain.AuthEntity;

namespace JWTPermissionBased.Application.Common.Interfaces;

public interface IAuthService
{
    Task<JwtAuthResultDto> AuthenticationToken(User userAuth);
}