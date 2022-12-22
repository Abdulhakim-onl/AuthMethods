using System.Text.Json.Serialization;

namespace JWTPermissionBased.Application.Common.Models;

public class JwtAuthResultDto
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("temporaryToken")]
    public string? TemporaryToken { get; set; }
}