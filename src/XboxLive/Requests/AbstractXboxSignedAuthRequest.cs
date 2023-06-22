using System;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using XboxAuthNet.XboxLive.Crypto;

namespace XboxAuthNet.XboxLive.Requests
{
    public abstract class AbstractXboxSignedAuthRequest : AbstractXboxAuthRequest
    {
        public AbstractXboxSignedAuthRequest()
        {
            ContractVersion = "2";
            Signer = new XboxRequestSigner(new ECDCertificatePopCryptoProvider());
        }
        
        public IXboxRequestSigner Signer { get; set; }
        
        protected abstract string RequestUrl { get; }
        protected virtual string Token { get; } = "";
        protected abstract object BuildBody();

        protected override HttpRequestMessage BuildRequest()
        {
            var body = BuildBody();
            var bodyStr = JsonSerializer.Serialize(body);

            var req = new HttpRequestMessage
            {
                RequestUri = new Uri(RequestUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(bodyStr, Encoding.UTF8, "application/json")
            };
                
            var signature = Signer.SignRequest(RequestUrl, Token, bodyStr);
            req.Headers.Add("Signature", signature);
            AddDefaultHeaders(req);
            return req;
        }
    }
}