using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests
{
    public class XboxTitleTokenRequest : AbstractXboxSignedAuthRequest
    {
        public string? AccessToken { get; set; }
        public string? DeviceToken { get; set; }
        public string? TokenPrefix { get; set; } = XboxTokenPrefix;
        public string? RelyingParty { get; set; } = XboxAuthRelyingParty;

        protected override string RequestUrl => "https://title.auth.xboxlive.com/title/authenticate";
        protected override object BuildBody()
        {
            if (string.IsNullOrEmpty(AccessToken))
                throw new InvalidOperationException("AccessToken was null");
            if (string.IsNullOrEmpty(RelyingParty))
                throw new InvalidOperationException("RelyingParty was null");
                
            return new 
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    DeviceToken = DeviceToken,
                    RpsTicket = TokenPrefix + AccessToken,
                    SiteName = "user.auth.xboxlive.com",
                    ProofKey = Signer.ProofKey,
                },
                RelyingParty = RelyingParty,
                TokenType = "JWT"
            };
        }

        public Task<XboxAuthResponse> Send(HttpClient httpClient)
        {
            return Send<XboxAuthResponse>(httpClient);
        }
    }
}