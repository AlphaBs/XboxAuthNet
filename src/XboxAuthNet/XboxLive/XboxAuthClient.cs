using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Requests;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive
{
    // https://github.com/PrismarineJS/prismarine-auth/blob/master/src/TokenManagers/XboxTokenManager.js
    public class XboxAuthClient
    {
        public const string XboxScope = "service::user.auth.xboxlive.com::MBI_SSL";

        private readonly HttpClient _httpClient;

        public XboxAuthClient(HttpClient httpClient) => 
            _httpClient = httpClient;

        public Task<XboxAuthResponse> RequestUserToken(string rps) => 
            RequestUserToken(new XboxUserTokenRequest()
            {
                AccessToken = rps
            });

        public Task<XboxAuthResponse> RequestUserToken(XboxUserTokenRequest request) =>
            request.Send(_httpClient);

        public Task<XboxAuthResponse> RequestSignedUserToken(string rps) =>
            RequestSignedUserToken(new XboxSignedUserTokenRequest()
            {
                AccessToken = rps
            });

        public Task<XboxAuthResponse> RequestSignedUserToken(XboxSignedUserTokenRequest request) =>
            request.Send(_httpClient);

        public Task<XboxAuthResponse> RequestDeviceToken(string deivceType, string deviceVersion) =>
            RequestDeviceToken(new XboxDeviceTokenRequest()
            {
                DeviceType = deivceType,
                DeviceVersion = deviceVersion
            });

        public Task<XboxAuthResponse> RequestDeviceToken(XboxDeviceTokenRequest request) =>
            request.Send(_httpClient);
        
        public Task<XboxAuthResponse> RequestTitleToken(string accessToken, string deviceToken) =>
            RequestTitleToken(new XboxTitleTokenRequest()
            {
                AccessToken = accessToken,
                DeviceToken = deviceToken
            });

        public Task<XboxAuthResponse> RequestTitleToken(XboxTitleTokenRequest request) =>
            request.Send(_httpClient);

        public Task<XboxAuthResponse> RequestXsts(string userToken) =>
            RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken
            });

        public Task<XboxAuthResponse> RequestXsts(string userToken, string relyingParty) =>
            RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken,
                RelyingParty = relyingParty
            });

        public Task<XboxAuthResponse> RequestXsts(XboxXstsRequest request) =>
            request.Send(_httpClient);

        public Task<XboxSisuResponse> SisuAuth(XboxSisuAuthRequest request) =>
            request.Send(_httpClient);
    }
}
