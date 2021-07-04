using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
// https://docs.microsoft.com/en-us/advertising/guides/authentication-oauth
// https://docs.microsoft.com/en-us/advertising/shopping-content/code-example-authentication-oauth

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuth
    {
        public MicrosoftOAuth(string clientId, string scope)
        {
            this.ClientId = clientId;
            this.Scope = scope;
        }

        protected const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
        protected const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";
        protected const string OAuthDesktopPath = "/oauth20_desktop.srf";
        protected const string OAuthErrorPath = "/err.srf";
        protected const string OAuthToken = "https://login.live.com/oauth20_token.srf";

        public string ClientId { get; private set; }
        public string Scope { get; private set; }

        public MicrosoftOAuthAuthCode? AuthCode { get; protected set; }

        protected Dictionary<string, string> createCommonQueriesForAuth(string scope, string redirectUrl = OAuthDesktop)
        {
            return new Dictionary<string, string>()
            {
                { "client_id", ClientId },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "scope", scope }
            };
        }

        public string CreateUrl()
        {
            return CreateUrl(null, null);
        }

        public string CreateUrl(string? redirect, string? state)
        {
            var url = OAuthAuthorize;
            var query = createCommonQueriesForAuth(Scope);
            query["response_type"] = "code";

            if (!string.IsNullOrEmpty(redirect))
                query["redirect_uri"] = redirect;

            if (!string.IsNullOrEmpty(state))
                query["state"] = state;

            return url + "?" + HttpUtil.GetQueryString(query);
        }

        public bool CheckLoginSuccess(string url)
        {
            var uri = new Uri(url);
            return CheckLoginSuccess(uri);
        }

        public bool CheckLoginSuccess(Uri uri)
        {
            try
            {
                var query = HttpUtil.ParseQuery(uri.Query);
                var authcode = new MicrosoftOAuthAuthCode
                {
                    Code = query["code"],
                    Error = query["error"],
                    ErrorDescription = HttpUtil.UrlDecode(query["error_description"])
                };

                this.AuthCode = authcode;
                return authcode.IsSuccess;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public bool TryGetTokens(out MicrosoftOAuthResponse? response, string? refreshToken = null)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                if (AuthCode?.IsSuccess ?? false)
                {
                    response = GetTokens();
                    return response.IsSuccess;
                }
                else
                {
                    response = null;
                    return false;
                }
            }

            response = RefreshToken(refreshToken);
            return response.IsSuccess;
        }

        public MicrosoftOAuthResponse GetTokens()
        {
            if (AuthCode == null || !AuthCode.IsSuccess)
                throw new InvalidOperationException("AuthCode.IsSuccess was not true. Create AuthCode first.");

            var url = OAuthToken;
            var query = createCommonQueriesForAuth(Scope);
            query["code"] = AuthCode.Code ?? "";

            var req = HttpUtil.CreateDefaultRequest(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            HttpUtil.WriteRequest(req, HttpUtil.GetQueryString(query));

            var res = req.GetResponse();
            var resBody = HttpUtil.ReadResponse(res);

            return JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(resBody);
        }

        public MicrosoftOAuthResponse RefreshToken(string refreshToken)
        {
            var url = OAuthToken;
            var query = createCommonQueriesForAuth(Scope);
            query["refresh_token"] = refreshToken;
            query["grant_type"] = "refresh_token";

            var req = HttpUtil.CreateDefaultRequest(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            HttpUtil.WriteRequest(req, HttpUtil.GetQueryString(query));

            var res = req.GetResponse();
            var resBody = HttpUtil.ReadResponse(res);

            return JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(resBody);
        }

        public static string GetSignOutUrl()
        {
            return "https://login.microsoftonline.com/consumer/oauth2/v2.0/logout";
        }
    }
}
