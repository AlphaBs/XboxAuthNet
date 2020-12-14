using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace XboxAuthNet.OAuth
{
    // core/live/config.ts
    // core/live/index.ts
    public class MicrosoftOAuth
    {
        const string MyXboxLive = "0000000048093EE3";
        const string XboxApp = "000000004C12AE6F";

        const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
        const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";
        const string OAuthToken = "https://login.live.com/oauth20_token.srf";

        public string ClientId { get; set; } = XboxApp;

        Lazy<Regex> FTagRegex = new Lazy<Regex>(() 
            => new Regex(@"sFTTag:'.*value=\""(.*)\""\/>'"));

        Lazy<Regex> UrlPostRegex = new Lazy<Regex>(()
            => new Regex(@"urlPost:'(.+?(?=\'))"));

        Lazy<Regex> IdentityConfirmationRegex = new Lazy<Regex>(() 
            => new Regex(@"id=\""fmHF\"" action=\""(.*?)\"""));

        Lazy<Regex> ActivityConfirmationRegex = new Lazy<Regex>(()
            => new Regex(@"identity\/confirm"));

        public string CreateUrl()
        {
            return CreateUrl(OAuthDesktop, null);
        }

        public string CreateUrl(string redirectUrl, string state)
        {
            var url = OAuthAuthorize;
            var query = createCommonQueriesForAuth(OAuthDesktop);
            query["response_type"] = "code";

            if (!string.IsNullOrEmpty(state))
                query["state"] = state;

            return url + "?" + HttpUtil.GetQueryString(query);
        }

        public string CheckLoginSuccess(string url)
        {
            var uri = new Uri(url);
            var query = HttpUtil.ParseQuery(uri.Query);
            var code = query["code"];

            return code;

            if (string.IsNullOrEmpty(code))
                return null;

            return code.Split('.').Last();
        }

        private Dictionary<string, string> createCommonQueriesForAuth(string redirectUrl)
        {
            return new Dictionary<string, string>()
            {
                { "client_id", ClientId },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
                { "scope", "service::user.auth.xboxlive.com::MBI_SSL" }
            };
        }

        public MicrosoftOAuthResponse GetAuthToken(string code, string redirectUrl=OAuthDesktop)
        {
            var url = OAuthToken;
            var query = createCommonQueriesForAuth(redirectUrl);
            query["code"] = code;

            var req = HttpUtil.CreateDefaultRequest(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            HttpUtil.WriteRequest(req, HttpUtil.GetQueryString(query));

            var res = req.GetResponse();
            var resBody = HttpUtil.ReadResponse(res);

            return JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(resBody);
        }

        public MicrosoftOAuthResponse RefreshToken(string refreshToken, string redirectUrl=OAuthDesktop)
        {
            var url = OAuthToken;
            var query = createCommonQueriesForAuth(redirectUrl);
            query["refresh_token"] = refreshToken;

            var req = HttpUtil.CreateDefaultRequest(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            HttpUtil.WriteRequest(req, HttpUtil.GetQueryString(query));

            var res = req.GetResponse();
            var resBody = HttpUtil.ReadResponse(res);

            return JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(resBody);
        }

        public MicrosoftOAuthResponse AutoLogin(string email, string password)
        {
            var pre = AutoLoginPreAuth();
            return AutoLoginUrlPost(email, password, pre);
        }

        private PreAuthResponse AutoLoginPreAuth()
        {
            try
            {
                var url = OAuthAuthorize;
                var query = createCommonQueriesForAuth(OAuthDesktop);
                query["display"] = "touch";
                query["locale"] = "en";
                query["response_type"] = "token";

                var req = HttpUtil.CreateDefaultRequest(url, query);
                var res = req.GetResponseNoException();
                if ((int)res.StatusCode / 100 != 2)
                    throw new XboxAuthException("Pre-authentication failed to request.", null, res);

                // parse only key=value. remove domain, Secure, path, HttpOnly
                var setCookie = res.Headers["Set-Cookie"];
                var cookie = string.Join(";", setCookie.Split(',').Select(x => x.Split(';')[0]));

                var body = HttpUtil.ReadResponse(res);

                var ppft = getMatchIndex(body, FTagRegex.Value, 1);
                if (ppft == null)
                    throw new XboxAuthException("Could not match 'PPFT' parameter, " +
                       $"please fill an issue on ${IssueLink.CreateIssue}", IssueLink.CreateIssue);

                var urlPost = getMatchIndex(body, UrlPostRegex.Value, 1);
                if (urlPost == null)
                    throw new XboxAuthException("Could not match 'urlPost' parameter, " +
                       $"please fill an issue on ${IssueLink.CreateIssue}", IssueLink.CreateIssue);

                return new PreAuthResponse(cookie, ppft, urlPost);
            }
            catch (XboxAuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new XboxAuthException("Failed to " + nameof(AutoLoginPreAuth), null, ex);
            }
        }

        private MicrosoftOAuthResponse AutoLoginUrlPost(string email, string password, PreAuthResponse pre)
        {
            try
            {
                var url = pre.UrlPost;
                Dictionary<string, string> query = new Dictionary<string, string>
                {
                    ["login"] = email,
                    ["loginfmt"] = email,
                    ["passwd"] = password,
                    ["PPFT"] = pre.PPFT
                };

                var req = HttpUtil.CreateDefaultRequest(url);
                req.Method = "POST";
                req.AllowAutoRedirect = true;
                req.MaximumAutomaticRedirections = 1;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Headers[HttpRequestHeader.Cookie] = pre.Cookie;

                HttpUtil.WriteRequest(req, HttpUtil.GetQueryString(query));

                var res = req.GetResponseNoException();
                if ((int)res.StatusCode / 100 != 2)
                    throw new XboxAuthException("Authentication failed to request.", null, res);

                var responseUri = res.ResponseUri.OriginalString;
                if (responseUri == pre.UrlPost)
                    throw new XboxAuthException("Invalid credentials.", null);

                var uriSplit = responseUri.Split('#');
                if (uriSplit.Length <= 1 || string.IsNullOrEmpty(uriSplit[1]))
                {
                    var body = HttpUtil.ReadResponse(res);
                    Console.WriteLine(body);

                    if (requiresIdentityConfirmation(body))
                        throw new XboxAuthException("Activity confirmation required, please refer to " +
                            IssueLink.UnauthorizedActivityError, 
                            IssueLink.UnauthorizedActivityError);
                    else
                        throw new XboxAuthException("Invalid credentials or 2FA enabled, please refer to " +
                            IssueLink.TwoFactorAuthenticationError, 
                            IssueLink.TwoFactorAuthenticationError);
                }

                var hash = uriSplit[1];
                var qs = HttpUtil.ParseQuery(hash);

                return new MicrosoftOAuthResponse()
                {
                    AccessToken = qs["access_token"],
                    TokenType = qs["token_type"],
                    ExpireIn = qs["expire_in"],
                    Scope = qs["scope"],
                    RefreshToken = qs["refresh_token"],
                    UserId = qs["user_id"]
                };
            }
            catch (XboxAuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new XboxAuthException("Failed to " + nameof(AutoLoginUrlPost), null, ex);
            }
        }

        private bool requiresIdentityConfirmation(string body)
        {
            var action = getMatchIndex(body, IdentityConfirmationRegex.Value, 1);
            if (action == null)
                return false;
            else
                return getMatchIndex(action, ActivityConfirmationRegex.Value, 0) != null;
        }

        private string getMatchIndex(string input, Regex r, int i)
        {
            var match = r.Match(input);
            if (!match.Success || match.Groups.Count <= i)
                return null;
            else
                return match.Groups[i].Value;
        }
    }
}
