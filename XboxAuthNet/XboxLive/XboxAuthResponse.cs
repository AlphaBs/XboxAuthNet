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
        public const string NoXboxAccountError = "2148916233";
        public const string ChildError = "2148916238";

        public XboxAuthResponse()
        {

        }

        public XboxAuthResponse(bool result)
        {
            this.result = result;
            this.useResult = true;
        }

        private bool result;
        private bool useResult = false;

        [JsonIgnore]
        public bool IsSuccess => useResult ? result : string.IsNullOrEmpty(Error);

        public string? UserXUID { get; set; }
        public string? UserHash { get; set; }
        
        [JsonPropertyName("IssueInstant")]
        public string? IssueInstant { get; set; }

        [JsonPropertyName("Token")]
        public string? Token { get; set; }

        [JsonPropertyName("NotAfter")]
        public string? ExpireOn { get; set; }

        [JsonPropertyName("XErr")]
        public string? Error { get; set; }

        [JsonPropertyName("Message")]
        public string? Message { get; set; }

        [JsonPropertyName("Redirect")]
        public string? Redirect { get; set; }
    }
}
