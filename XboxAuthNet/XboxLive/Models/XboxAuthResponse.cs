using System;
using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Models
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

        /// <summary>
        /// checks token is not null and not empty, checks token is not expired
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(ExpireOn))
                return false;

            if (DateTime.Parse(ExpireOn) < DateTime.Now)
                return false;

            if (string.IsNullOrEmpty(Token))
                return false;

            return true;
        }
    }
}
