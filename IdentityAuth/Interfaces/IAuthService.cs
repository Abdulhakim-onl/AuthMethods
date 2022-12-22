using IdentityAuth.Models;
using System.Security.Claims;

namespace IdentityAuth.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateTokenOptions(List<Claim> claims);
        Task<List<Claim>> GetClaims(User user);
    }
}
