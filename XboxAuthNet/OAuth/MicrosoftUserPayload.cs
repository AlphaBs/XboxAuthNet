using System.Text.Json.Serialization;

namespace XboxAuthNet.OAuth
{
    // https://docs.microsoft.com/en-us/azure/active-directory/develop/id-tokens#payload-claims
    public class MicrosoftUserPayload
    {
        [JsonPropertyName("iss")]
        public string? Issuer { get; set; }

        [JsonPropertyName("sub")]
        public string? Subject { get; set; }

        [JsonPropertyName("oid")]
        public string? UserId { get; set; }

        [JsonPropertyName("aud")]
        public string? ApplicationId { get; set; }

        [JsonPropertyName("exp")]
        public long ExpiresOn { get; set; }

        [JsonPropertyName("iat")]
        public long IssuedAt { get; set; }

        [JsonPropertyName("nbf")]
        public long NotBefore { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("preferred_username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
