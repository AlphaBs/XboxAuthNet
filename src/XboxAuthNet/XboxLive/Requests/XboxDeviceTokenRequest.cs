using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests
{
    public class XboxDeviceTokenRequest : AbstractXboxSignedAuthRequest
    {
        public string? Id { get; set; }
        public string? SerialNumber { get; set; }
        public string? DeviceType { get; set; }
        public string? DeviceVersion { get; set; }
        public string? RelyingParty { get; set; } = XboxAuthConstants.XboxAuthRelyingParty;

        protected override string RequestUrl => "https://device.auth.xboxlive.com/device/authenticate";
        protected override object BuildBody()
        {
            if (string.IsNullOrEmpty(DeviceType))
                throw new InvalidOperationException("DeviceType was null");
            if (string.IsNullOrEmpty(DeviceVersion))
                throw new InvalidOperationException("DeviceVersion was null");
            if (string.IsNullOrEmpty(RelyingParty))
                throw new InvalidOperationException("RelyingParty was null");

            return new
            {
                Properties = new
                {
                    AuthMethod = "ProofOfPossession",
                    Id = "{" + Id ?? nextUUID() + "}",
                    DeviceType = DeviceType,
                    SerialNumber = "{" + SerialNumber ?? nextUUID() + "}",
                    Version = DeviceVersion,
                    ProofKey = Signer.ProofKey
                },
                RelyingParty = RelyingParty,
                TokenType = "JWT"
            };
        }

        private string nextUUID()
        {
            return Guid.NewGuid().ToString();
        }

        public Task<XboxAuthResponse> Send(HttpClient httpClient)
        {
            return Send<XboxAuthResponse>(httpClient);
        }
    }
}