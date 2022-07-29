using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XboxAuthNet.XboxLive
{
    public class XboxAuthResponse
    {
        // for backward compatibility
        public string? UserXUID => XuiClaims?.XboxUserId;
        public string? UserHash => XuiClaims?.UserHash;

        public XboxAuthXuiClaims? XuiClaims { get; set; }

        [JsonPropertyName("IssueInstant")]
        public string? IssueInstant { get; set; }

        [JsonPropertyName("Token")]
        public string? Token { get; set; }

        [JsonPropertyName("NotAfter")]
        public string? ExpireOn { get; set; }
    }
}
