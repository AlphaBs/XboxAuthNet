using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public string ExpireIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("refresh_token")]
        private string rawRefreshToken { get; set; }

        public string RefreshToken
        {
            get => rawRefreshToken.Split('.').Last();
            set => rawRefreshToken = "M.R3_BAY." + value;
        }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
