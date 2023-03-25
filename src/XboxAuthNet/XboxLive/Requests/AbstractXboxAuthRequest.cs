using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests
{
    public abstract class AbstractXboxAuthRequest
    {
        public const string XboxLiveRelyingParty = "http://xboxlive.com";
        public const string XboxAuthRelyingParty = "http://auth.xboxlive.com";
        public const string AzureTokenPrefix = "d=";
        public const string XboxTokenPrefix = "t=";

        public AbstractXboxAuthRequest()
        {
            ResponseHandler = new XboxAuthResponseHandler();
        }

        public XboxAuthResponseHandler? ResponseHandler { get; set; }
        public string? ContractVersion { get; set; }

        protected void AddDefaultHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("User-Agent", HttpUtil.UserAgent);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Cache-Control", "no-store, must-revalidate, no-cache");
            request.Headers.Add("x-xbl-contract-version", ContractVersion ?? "");
        }

        protected abstract HttpRequestMessage BuildRequest();

        protected async Task<T> Send<T>(HttpClient httpClient)
        {
            if (ResponseHandler == null)
                throw new InvalidOperationException("ResponseHandler was null");

            var request = BuildRequest();
            var response = await httpClient.SendAsync(request);
            return await ResponseHandler.HandleResponse<T>(response);
        }
    }
}