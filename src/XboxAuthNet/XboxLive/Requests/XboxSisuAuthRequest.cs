using System;
using System.Threading.Tasks;
using System.Net.Http;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests
{
    public class XboxSisuAuthRequest : AbstractXboxSignedAuthRequest
    {
        public string? TokenPrefix { get; set; } = XboxAuthConstants.XboxTokenPrefix;
        public string? AccessToken { get; set; }
        public string? RelyingParty { get; set; } = XboxAuthConstants.XboxLiveRelyingParty;
        public string? ClientId { get; set; }
        public string? DeviceToken { get; set; }

        protected override string RequestUrl => "https://sisu.xboxlive.com/authorize";
        protected override object BuildBody()
        { 
            if (string.IsNullOrEmpty(AccessToken))
                throw new InvalidOperationException("AccessToken was null");
            if (string.IsNullOrEmpty(RelyingParty))
                throw new InvalidOperationException("RelyingParty was null");
                
            return new
            {
                AccessToken = TokenPrefix + AccessToken,
                AppId = ClientId,
                DeviceToken = DeviceToken,
                Sandbox = "RETAIL",
                UseModernGamertag = true,
                SiteName = "user.auth.xboxlive.com",
                RelyingParty = RelyingParty,
                ProofKey = Signer.ProofKey
            };
        }

        public Task<XboxSisuResponse> Send(HttpClient httpClient)
        {
            return Send<XboxSisuResponse>(httpClient);
        }
    }
}