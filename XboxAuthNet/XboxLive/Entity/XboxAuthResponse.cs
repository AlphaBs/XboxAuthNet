using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Entity
{
    public class XboxAuthResponse
    {
        // for backward compatibility
        public string? UserXUID => XuiClaims?.XboxUserId;
        public string? UserHash => XuiClaims?.UserHash;

        [JsonPropertyName("DisplayClaims")]
        [JsonConverter(typeof(XboxAuthXuiClaimsJsonConverter))]
        public XboxAuthXuiClaims? XuiClaims { get; set; }

        [JsonPropertyName("IssueInstant")]
        public string? IssueInstant { get; set; }

        [JsonPropertyName("Token")]
        public string? Token { get; set; }

        [JsonPropertyName("NotAfter")]
        public string? ExpireOn { get; set; }
    }
}
