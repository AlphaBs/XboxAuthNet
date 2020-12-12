using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace XboxAuthNet.OAuth
{
    // core/live/config.ts
    // core/live/index.ts
    public class XboxLiveOAuth
    {
        const string MyXboxLive = "0000000048093EE3";
        const string XboxApp = "000000004C12AE6F";

        Lazy<Regex> FTagRegex = new Lazy<Regex>(() 
            => new Regex(@"sFTTag:'.*value=\""(.*)\""\/>'"));

        Lazy<Regex> UrlPostRegex = new Lazy<Regex>(()
            => new Regex(@"urlPost:'(.+?(?=\'))"));

        public PreAuthResponse PreAuth()
        {
            var url = "https://login.live.com/oauth20_authorize.srf";
            Dictionary<string, string> query = new Dictionary<string, string>
            {
                ["client_id"] = XboxApp,
                ["redirect_uri"] = "https://login.live.com/oauth20_desktop.srf",
                ["scope"] = "service::user.auth.xboxlive.com::MBI_SSL",
                ["display"] = "touch",
                ["response_type"] = "token",
                ["locale"] = "en"
            };

            var req = HttpUtil.CreateDefaultRequest(url, query);
            var res = (HttpWebResponse)req.GetResponse();

            // parse only key=value. remove domain, Secure, path, HttpOnly
            var setCookie = res.Headers["Set-Cookie"];
            var cookie = string.Join(";", setCookie.Split(',').Select(x => x.Split(';')[0]));

            var body = HttpUtil.ReadResponse(res);

            var ppft = getMatchIndex(body, FTagRegex.Value, 1);
            if (ppft == null)
                throw new Exception();

            var urlPost = getMatchIndex(body, UrlPostRegex.Value, 1);
            if (urlPost == null)
                throw new Exception();

            return new PreAuthResponse(cookie, ppft, urlPost);
        }

        public LogUserResponse LogUser(string email, string password, PreAuthResponse pre)
        {
            var url = pre.UrlPost;
            Dictionary<string, string> query = new Dictionary<string, string>
            {
                ["login"]    = email,
				["loginfmt"] = email,
				["passwd"]   = password,
				["PPFT"]     = pre.PPFT
            };

            var req = HttpUtil.CreateDefaultRequest(url);
            req.Method = "POST";
            req.AllowAutoRedirect = true;
            req.MaximumAutomaticRedirections = 1;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Headers[HttpRequestHeader.Cookie] = pre.Cookie;

            HttpUtil.WriteRequest(req, HttpUtil.GetQueryString(query));

            var res = req.GetResponse();
            var body = HttpUtil.ReadResponse(res);

            var responseUri = res.ResponseUri.OriginalString;
            if (responseUri == pre.UrlPost)
                throw new Exception();

            var uriSplit = responseUri.Split('#');
            if (uriSplit.Length <= 1 || string.IsNullOrEmpty(uriSplit[1]))
                throw new Exception();

            var hash = uriSplit[1];
            var qs = HttpUtil.ParseQuery(hash);

            return new LogUserResponse()
            {
                AccessToken = qs["access_token"],
                TokenType = qs["token_type"],
                ExpireIn = qs["expire_in"],
                Scope = qs["scope"],
                RefreshToken = qs["refresh_token"],
                UserId = qs["user_id"]
            };
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
