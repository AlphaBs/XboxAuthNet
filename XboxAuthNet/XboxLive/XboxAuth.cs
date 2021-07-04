using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

//var rpsRes = exchanger.ExchangeRpsTicketForUserToken(AccessToken);
//var xstsRes = exchanger.ExchangeTokensForXSTSIdentity(rpsRes.Token, null, null, XSTSRelyingParty, null);

namespace XboxAuthNet.XboxLive
{
    public class XboxAuth
    {
        public const string XboxScope = "service::user.auth.xboxlive.com::MBI_SSL";

        const string UserAuthenticate = "https://user.auth.xboxlive.com/user/authenticate";
        const string XstsAuthorize = "https://xsts.auth.xboxlive.com/xsts/authorize";
        const string DefaultRelyingParty = "http://xboxlive.com";

        public XboxAuthResponse ExchangeRpsTicketForUserToken(string rps)
        {
            try
            {
                var req = HttpUtil.CreateDefaultRequest(UserAuthenticate);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Accept = "application/json";
                req.Headers["x-xbl-contract-version"] = "0";

                var reqBody = new JObject()
                {
                    { "RelyingParty", "http://auth.xboxlive.com" },
                    { "TokenType", "JWT" },
                    { "Properties", new JObject()
                        {
                            { "AuthMethod", "RPS" },
                            { "SiteName", "user.auth.xboxlive.com" },
                            { "RpsTicket", rps }
                        }
                    }
                };
                HttpUtil.WriteRequest(req, reqBody.ToString());

                var res = req.GetResponseNoException();
                var body = HttpUtil.ReadResponse(res);

                return parseAuthResponse(body);
            }
            catch (XboxAuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new XboxAuthException("Failed to " + nameof(ExchangeRpsTicketForUserToken),  null, ex);
            }
        }

        public XboxAuthResponse ExchangeTokensForXstsIdentity(string userToken, string? deviceToken, string? titleToken,
            string? xstsRelyingParty, string[]? optionalDisplayClaims)
        {
            try
            {
                var req = HttpUtil.CreateDefaultRequest(XstsAuthorize);
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Accept = "application/json";
                req.Headers["x-xbl-contract-version"] = "1";

                var reqBody = JObject.FromObject(new
                {
                    RelyingParty = xstsRelyingParty ?? DefaultRelyingParty,
                    TokenType = "JWT",
                    Properties = new
                    {
                        UserTokens = nullHandle(userToken),
                        DeviceToken = deviceToken,
                        TitleToken = titleToken,
                        OptionalDisplayClaims = nullHandle(optionalDisplayClaims),
                        SandboxId = "RETAIL"
                    }
                },
                new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                HttpUtil.WriteRequest(req, reqBody.ToString());

                var res = req.GetResponseNoException();
                var body = HttpUtil.ReadResponse(res);

                return parseAuthResponse(body);
            }
            catch (XboxAuthException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new XboxAuthException("Failed to" + nameof(ExchangeTokensForXstsIdentity), null, ex);
            }
        }

        private XboxAuthResponse parseAuthResponse(string json)
        {
            var job = JObject.Parse(json);
            var authres = job.ToObject<XboxAuthResponse>();
            if (authres == null)
                return new XboxAuthResponse();

            authres.UserXUID = job["DisplayClaims"]?["xui"]?[0]?["xid"]?.ToString();
            authres.UserHash = job["DisplayClaims"]?["xui"]?[0]?["uhs"]?.ToString();

            return authres;
        }

        private JArray? nullHandle(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            else
                return new JArray(str);
        }

        private JArray? nullHandle(object[]? objs)
        {
            if (objs == null || objs.Length == 0)
                return null;
            else
                return new JArray(objs);
        }
    }
}
