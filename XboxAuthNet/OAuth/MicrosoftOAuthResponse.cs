using System.Linq;
using System.Text.Json.Serialization;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpireIn { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RawRefreshToken { get; set; }

        [JsonIgnore]
        public string? RefreshToken
        {
            get => RawRefreshToken?.Split('.')?.Last();
            set => RawRefreshToken = "M.R3_BAY." + value;
        }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }

        [JsonPropertyName("error_codes")]
        public int[]? ErrorCodes { get; set; }
    }
}
