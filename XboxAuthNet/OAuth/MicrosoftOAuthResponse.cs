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
        [JsonIgnore]
        public bool IsSuccess => string.IsNullOrEmpty(this.Error);

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpireIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("refresh_token")]
        private string rawRefreshToken { get; set; }

        [JsonIgnore]
        public string RefreshToken
        {
            get => rawRefreshToken?.Split('.')?.Last();
            set => rawRefreshToken = "M.R3_BAY." + value;
        }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("error_codes")]
        public int[] ErrorCodes { get; set; }
    }
}
