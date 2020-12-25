using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XboxAuthNet.XboxLive
{
    public class XboxAuthResponse
    {
        public const string NoXboxAccountError = "2148916233";
        public const string ChildError = "2148916238";

        [JsonIgnore]
        public bool IsSuccess => string.IsNullOrEmpty(Error);

        public string UserXUID { get; set; }
        public string UserHash { get; set; }
        
        [JsonProperty("IssueInstant")]
        public string IssueInstant { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("NotAfter")]
        public string ExpireOn { get; set; }

        [JsonProperty("XErr")]
        public string Error { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Redirect")]
        public string Redirect { get; set; }
    }
}
