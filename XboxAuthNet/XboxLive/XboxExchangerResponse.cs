using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XboxAuthNet.XboxLive
{
    public class XboxExchangerResponse
    {
        [JsonProperty]
        public string IssueInstant { get; private set; }
        [JsonProperty]
        public string NotAfter { get; private set; }
        [JsonProperty]
        public string Token { get; private set; }
    }
}
