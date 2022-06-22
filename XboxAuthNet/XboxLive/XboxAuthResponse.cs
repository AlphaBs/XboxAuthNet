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
        public string? UserXUID { get; set; }
        public string? UserHash { get; set; }
        
        [JsonPropertyName("IssueInstant")]
        public string? IssueInstant { get; set; }

        [JsonPropertyName("Token")]
        public string? Token { get; set; }

        [JsonPropertyName("NotAfter")]
        public string? ExpireOn { get; set; }
    }
}
