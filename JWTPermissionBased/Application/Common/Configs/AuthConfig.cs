using System.Text.Json.Serialization;

namespace JWTPermissionBased.Application.Common.Configs
{
    public class GoogleExternalOptions
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("clientSecret")]
        public string ClientSecret { get; set; }
    }
}
