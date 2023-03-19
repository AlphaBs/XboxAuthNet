using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Models;
using XboxAuthNet.XboxLive.Pop;

namespace XboxAuthNet.XboxLive
{
    // https://github.com/PrismarineJS/prismarine-auth/blob/master/src/TokenManagers/XboxTokenManager.js
    public class XboxSecureAuth
    {
        public const string AzureTokenPrefix = "d=";
        public const string XboxTokenPrefix = "t=";

        public static XboxSecureAuth Create(HttpClient httpClient)
        {
            return new XboxSecureAuth(
                httpClient, 
                new XboxSisuRequestSigner(new DefaultECSigner()));
        }

        const string SisuAuthorize = "https://sisu.xboxlive.com/authorize";
        const string DeviceAuth = "https://device.auth.xboxlive.com/device/authenticate";
        const string TitleAuth = "https://title.auth.xboxlive.com/title/authenticate";

        private readonly HttpClient _httpClient;
        private readonly IXboxSisuRequestSigner _signer;

        public XboxSecureAuth(
            HttpClient httpClient,
            IXboxSisuRequestSigner signer)
        {
            _httpClient = httpClient;
            _signer = signer;
        }

        public Task<XboxSisuResponse> SisuAuth(string accessToken, string clientId, string deviceToken, string relyingParty, string? tokenPrefix=XboxTokenPrefix)
        {
            return signAndRequest<XboxSisuResponse>(SisuAuthorize, new
            {
                AccessToken = tokenPrefix + accessToken,
                AppId = clientId,
                DeviceToken = deviceToken,
                Sandbox = "RETAIL",
                UseModernGamertag = true,
                SiteName = "user.auth.xboxlive.com",
                RelyingParty = relyingParty,
                ProofKey = _signer.ProofKey
            }, "");
        }

        public Task<XboxAuthResponse> RequestUserToken(string accessToken, string? tokenPrefix="t=")
        {
            return signAndRequest<XboxAuthResponse>("https://user.auth.xboxlive.com/user/authenticate", new
            {
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT",
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = tokenPrefix + accessToken
                }
            }, "");
        }

        public Task<XboxAuthResponse> RequestDeviceToken(string deviceType, string deviceVersion)
        {
            return RequestDeviceToken(nextUUID(), nextUUID(), deviceType, deviceVersion);
        }

        public Task<XboxAuthResponse> RequestDeviceToken(string id, string serialNumber, string deviceType, string deviceVersion)
        {
            return signAndRequest<XboxAuthResponse>(DeviceAuth, new
            {
                Properties = new
                {
                    AuthMethod = "ProofOfPossession",
                    Id = "{" + id + "}",
                    DeviceType = deviceType,
                    SerialNumber = "{" + serialNumber + "}",
                    Version = deviceVersion,
                    ProofKey = _signer.ProofKey
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            }, "");
        }

        public Task<XboxAuthResponse> RequestTitleToken(string accessToken, string deviceToken, string tokenPrefix = XboxTokenPrefix)
        {
            return signAndRequest<XboxAuthResponse>(TitleAuth, new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    DeviceToken = deviceToken,
                    RpsTicket = tokenPrefix + accessToken,
                    SiteName = "user.auth.xboxlive.com",
                    ProofKey = _signer.ProofKey,
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            }, "");
        }

        private async Task<T> signAndRequest<T>(string uri, object body, string token)
        {
            var bodyStr = JsonSerializer.Serialize(body);

            var reqMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = HttpMethod.Post,
                Content = new StringContent(bodyStr, Encoding.UTF8, "application/json")
            };

            if (token == null)
                token = "";
            var signature = _signer.SignRequest(uri, token, bodyStr);
            reqMessage.Headers.Add("Signature", signature);
            reqMessage.Headers.Add("x-xbl-contract-version", "2");

            return await XboxRequest.Send<T>(_httpClient, reqMessage);
        }

        private string nextUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
