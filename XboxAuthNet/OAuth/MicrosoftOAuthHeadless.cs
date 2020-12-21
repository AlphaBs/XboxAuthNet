using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthHeadless
    {
        public MicrosoftOAuthHeadless(string clientId, string scope)
        {
            this.ClientId = clientId;
            this.Scope = scope;
        }

        protected const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
        protected const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";

        public string ClientId { get; private set; }
        public string Scope { get; private set; }

        Lazy<Regex> FTagRegex = new Lazy<Regex>(()
            => new Regex(@"sFTTag:'.*value=\""(.*)\""\/>'"));

        Lazy<Regex> UrlPostRegex = new Lazy<Regex>(()
            => new Regex(@"urlPost:'(.+?(?=\'))"));

        Lazy<Regex> IdentityConfirmationRegex = new Lazy<Regex>(()
            => new Regex(@"id=\""fmHF\"" action=\""(.*?)\"""));

        Lazy<Regex> ActivityConfirmationRegex = new Lazy<Regex>(()
            => new Regex(@"identity\/confirm"));

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

        public MicrosoftOAuthResponse GetTokensHeadless(string email, string password)
        {
            var pre = AutoLoginPreAuth();
            return AutoLoginUrlPost(email, password, pre);
        }

        private PreAuthResponse AutoLoginPreAuth()
        {
            try
            {
                var url = OAuthAuthorize;
                var query = createCommonQueriesForAuth(this.Scope);
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

                int exp = 0;
                int.TryParse(qs["expire_in"], out exp);

                return new MicrosoftOAuthResponse()
                {
                    AccessToken = qs["access_token"],
                    TokenType = qs["token_type"],
                    ExpireIn = exp,
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
