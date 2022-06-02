using Microsoft.Extensions.Logging;
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
        protected const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
        protected const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";
        protected const string OAuthDesktopPath = "/oauth20_desktop.srf";
        protected const string OAuthErrorPath = "/err.srf";
        protected const string OAuthToken = "https://login.live.com/oauth20_token.srf";

        private readonly ILogger<MicrosoftOAuth>? logger;
        private readonly HttpClient httpClient;

        public MicrosoftOAuth(string clientId, string scope, HttpClient client, ILoggerFactory? logFactory = null)
        {
            this.ClientId = clientId;
            this.Scope = scope;
            this.httpClient = client;
            this.logger = logFactory?.CreateLogger<MicrosoftOAuth>();
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

            logger?.LogTrace("Request to {RequestUri}", req.RequestUri.ToString());

            var res = await httpClient.SendAsync(req)
                .ConfigureAwait(false);
            var resBody = await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            logger?.LogTrace("code: {Code}, body: {Body}", res.StatusCode, resBody);

            res.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<MicrosoftOAuthResponse>(resBody)
                ?? new MicrosoftOAuthResponse(result: false);
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

            logger?.LogTrace("AuthCode detected: {Code}, {Error}, {ErrorDescription}", 
                authCode.Code, authCode.Error, authCode.ErrorDescription);
            return authCode.IsSuccess;
        }

        public async Task<MicrosoftOAuthResponse> GetTokens(MicrosoftOAuthAuthCode authCode)
        {
            if (authCode == null || !authCode.IsSuccess)
                throw new InvalidOperationException("AuthCode.IsSuccess was not true. Create AuthCode first.");

            var query = createQueriesForAuth(Scope);
            query["code"] = authCode.Code ?? "";

            try
            {
                return await microsoftOAuthRequest(new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(OAuthToken),
                    Content = new FormUrlEncodedContent(query)
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new MicrosoftOAuthException("Failed to " + nameof(GetTokens), ex);
            }
        }

        public async Task<MicrosoftOAuthResponse> RefreshToken(string refreshToken)
        {
            var query = createQueriesForAuth(Scope);
            query["refresh_token"] = refreshToken;
            query["grant_type"] = "refresh_token";

            try
            {
                return await microsoftOAuthRequest(new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(OAuthToken),
                    Content = new FormUrlEncodedContent(query)
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new MicrosoftOAuthException("Failed to " + nameof(RefreshToken), ex);
            }
        }

        public static string GetSignOutUrl()
        {
            return "https://login.microsoftonline.com/consumer/oauth2/v2.0/logout";
        }
    }
}
