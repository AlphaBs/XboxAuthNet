using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using XboxAuthNet.OAuth.Models;

// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
// https://docs.microsoft.com/en-us/advertising/guides/authentication-oauth
// https://docs.microsoft.com/en-us/advertising/shopping-content/code-example-authentication-oauth

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthCodeApiClient
    {
        const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
        const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";
        const string OAuthDesktopPath = "/oauth20_desktop.srf";
        const string OAuthErrorPath = "/err.srf";
        const string OAuthToken = "https://login.live.com/oauth20_token.srf";

        protected readonly HttpClient httpClient;

        public MicrosoftOAuthCodeApiClient(string clientId, string scope, HttpClient client)
        {
            this.ClientId = clientId;
            this.Scope = scope;
            this.httpClient = client;
        }

        public string ClientId { get; }
        public string Scope { get; }

        protected Dictionary<string, string?> createQueriesForAuth(string redirectUrl = OAuthDesktop)
        {
            return new Dictionary<string, string?>()
            {
                { "client_id", ClientId },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "scope", Scope }
            };
        }

        private async Task<MicrosoftOAuthResponse> microsoftOAuthRequest(HttpRequestMessage req, 
            CancellationToken cancellationToken)
        {
            req.Headers.Add("User-Agent", HttpUtil.UserAgent);
            req.Headers.Add("Accept-Encoding", "gzip");
            req.Headers.Add("Accept-Language", "en-US");

            var res = await httpClient.SendAsync(req, cancellationToken)
                .ConfigureAwait(false);
            
            return await handleMicrosoftOAuthResponse(res);
        }

        internal async Task<MicrosoftOAuthResponse> handleMicrosoftOAuthResponse(HttpResponseMessage res)
        {
            var resBody = await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            try
            {
                res.EnsureSuccessStatusCode();
                var resObj = JsonSerializer.Deserialize<MicrosoftOAuthResponse>(resBody);
                if (resObj == null)
                    throw new MicrosoftOAuthException("Response was null", (int)res.StatusCode);

                if (resObj.ExpiresOn == default)
                    resObj.ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(resObj.ExpireIn);
                return resObj;
            }
            catch (Exception ex) when (
                ex is JsonException ||
                ex is HttpRequestException)
            {
                try
                {
                    throw MicrosoftOAuthException.FromResponseBody(resBody, (int)res.StatusCode);
                }
                catch (FormatException)
                {
                    throw new MicrosoftOAuthException($"{(int)res.StatusCode}: {res.ReasonPhrase}", (int)res.StatusCode);
                }
            }
        }

        public string CreateUrlForOAuth() => CreateUrlForOAuth(null);

        public string CreateUrlForOAuth(MicrosoftOAuthParameters? param)
        {
            if (param == null)
                param = new MicrosoftOAuthParameters();

            if (string.IsNullOrEmpty(param.ResponseType))
                param.ResponseType = "code";

            var query = createQueriesForAuth();
            var paramQuery = param.ToQueryDictionary();

            // overwrite `paramQuery`
            foreach (var kv in paramQuery)
                query[kv.Key] = kv.Value;

            return OAuthAuthorize + "?" + HttpUtil.GetQueryString(query);
        }

        public async Task<MicrosoftOAuthResponse> GetTokens(MicrosoftOAuthCode authCode, 
            CancellationToken cancellationToken)
        {
            if (authCode == null || !authCode.IsSuccess)
                throw new InvalidOperationException("AuthCode.IsSuccess was not true. Create AuthCode first.");

            var query = createQueriesForAuth();
            query["code"] = authCode.Code ?? "";

            var res = await microsoftOAuthRequest(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthToken),
                Content = new FormUrlEncodedContent(query!)
            }, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(res.IdToken))
                res.IdToken = authCode.IdToken;
            return res;
        }

        public async Task<MicrosoftOAuthResponse> RefreshToken(string refreshToken, 
            CancellationToken cancellationToken)
        {
            var query = createQueriesForAuth();
            query["refresh_token"] = refreshToken;
            query["grant_type"] = "refresh_token";

            return await microsoftOAuthRequest(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthToken),
                Content = new FormUrlEncodedContent(query!)
            }, cancellationToken).ConfigureAwait(false);
        }

        public string CreateUrlForSignout()
        {
            return "https://login.microsoftonline.com/consumer/oauth2/v2.0/logout";
        }
    }
}
