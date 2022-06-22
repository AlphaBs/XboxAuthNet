using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
// https://docs.microsoft.com/en-us/advertising/guides/authentication-oauth
// https://docs.microsoft.com/en-us/advertising/shopping-content/code-example-authentication-oauth

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuth
    {
        const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
        const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";
        const string OAuthDesktopPath = "/oauth20_desktop.srf";
        const string OAuthErrorPath = "/err.srf";
        const string OAuthToken = "https://login.live.com/oauth20_token.srf";

        protected readonly HttpClient httpClient;

        public MicrosoftOAuth(string clientId, string scope, HttpClient client)
        {
            this.ClientId = clientId;
            this.Scope = scope;
            this.httpClient = client;
        }

        public string ClientId { get; }
        public string Scope { get; }

        protected Dictionary<string, string?> createQueriesForAuth(string scope, string redirectUrl = OAuthDesktop)
        {
            return new Dictionary<string, string?>()
            {
                { "client_id", ClientId },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "scope", scope }
            };
        }

        private async Task<MicrosoftOAuthResponse> microsoftOAuthRequest(HttpRequestMessage req)
        {
            req.Headers.Add("User-Agent", HttpUtil.UserAgent);
            req.Headers.Add("Accept-Encoding", "gzip");
            req.Headers.Add("Accept-Language", "en-US");

            var res = await httpClient.SendAsync(req)
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

        public string CreateUrlForOAuth()
        {
            return CreateUrlForOAuth(null, null);
        }

        public string CreateUrlForOAuth(string? redirect, string? state)
        {
            var url = OAuthAuthorize;
            var query = createQueriesForAuth(Scope);
            query["response_type"] = "code";

            if (!string.IsNullOrEmpty(redirect))
                query["redirect_uri"] = redirect;

            if (!string.IsNullOrEmpty(state))
                query["state"] = state;
            
            return url + "?" + HttpUtil.GetQueryString(query);
        }

        public bool CheckLoginSuccess(string url, out MicrosoftOAuthAuthCode authCode)
        {
            var uri = new Uri(url);
            return CheckLoginSuccess(uri, out authCode);
        }

        public bool CheckLoginSuccess(Uri uri, out MicrosoftOAuthAuthCode authCode)
        {
            var query = HttpUtility.ParseQueryString(uri.Query);
            authCode = new MicrosoftOAuthAuthCode
            {
                Code = query["code"],
                Error = query["error"],
                ErrorDescription = HttpUtility.UrlDecode(query["error_description"])
            };

            return authCode.IsSuccess;
        }

        public async Task<MicrosoftOAuthResponse> GetTokens(MicrosoftOAuthAuthCode authCode)
        {
            if (authCode == null || !authCode.IsSuccess)
                throw new InvalidOperationException("AuthCode.IsSuccess was not true. Create AuthCode first.");

            var query = createQueriesForAuth(Scope);
            query["code"] = authCode.Code ?? "";

            return await microsoftOAuthRequest(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthToken),
                Content = new FormUrlEncodedContent(query)
            }).ConfigureAwait(false);
        }

        public async Task<MicrosoftOAuthResponse> RefreshToken(string refreshToken)
        {
            var query = createQueriesForAuth(Scope);
            query["refresh_token"] = refreshToken;
            query["grant_type"] = "refresh_token";

            return await microsoftOAuthRequest(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(OAuthToken),
                Content = new FormUrlEncodedContent(query)
            }).ConfigureAwait(false);
        }

        public static string GetSignOutUrl()
        {
            return "https://login.microsoftonline.com/consumer/oauth2/v2.0/logout";
        }
    }
}
