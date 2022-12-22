using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace JWTPermissionBased.Application.Common.Configs
{
    public class JwtTokenConfig
    {
        [JsonPropertyName("secret")]
        public string? Secret { get; set; }

        [JsonPropertyName("issuer")]
        public string? Issuer { get; set; }

        [JsonPropertyName("audience")]
        public string? Audience { get; set; }

        [JsonPropertyName("accessTokenExpiration")]
        public int AccessTokenExpiration { get; set; }

        [JsonPropertyName("refreshTokenExpiration")]
        public int RefreshTokenExpiration { get; set; }
        
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
        
        public SigningCredentials GetSigningCredentials(SymmetricSecurityKey key)
        {
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}
