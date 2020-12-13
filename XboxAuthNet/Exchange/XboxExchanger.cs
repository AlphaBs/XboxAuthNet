using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace XboxAuthNet.Exchange
{
    public class XboxExchanger
    {
        const string UserAuthenticate = "https://user.auth.xboxlive.com/user/authenticate";
        const string XSTSAuthorize = "https://xsts.auth.xboxlive.com/xsts/authorize";
        const string DefaultRelyingParty = "http://xboxlive.com";

        public XboxExchangerResponse ExchangeRpsTicketForUserToken(string rps)
        {
            var req = HttpUtil.CreateDefaultRequest(UserAuthenticate);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers[HttpRequestHeader.Accept] = "application/json";
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

            var res = req.GetResponse();
            var body = HttpUtil.ReadResponse(res);
            return JsonConvert.DeserializeObject<XboxExchangerResponse>(body);
        }

        public XboxAuthResponse ExchangeTokensForXSTSIdentity(string userToken, string deviceToken, string titleToken,
            string XSTSRelyingParty, string[] optionalDisplayClaims)
        {
            var req = HttpUtil.CreateDefaultRequest(XSTSAuthorize);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers[HttpRequestHeader.Accept] = "application/json";
            req.Headers["x-xbl-contract-version"] = "1";

            var reqBody = JObject.FromObject(new
            {
                RelyingParty = XSTSRelyingParty ?? DefaultRelyingParty,
                TokenType = "JWT",
                Properties = new
                {
                    UserTokens = NullHandle(userToken),
                    DeviceToken = deviceToken,
                    TitleToken = titleToken,
                    OptionalDisplayClaims = NullHandle(optionalDisplayClaims),
                    SandboxId = "RETAIL"
                }
            },
            new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            HttpUtil.WriteRequest(req, reqBody.ToString());

            var res = req.GetResponse();
            var body = HttpUtil.ReadResponse(res);
            var job = JObject.Parse(body);

            return new XboxAuthResponse()
            {
                UserXUID = job["DisplayClaims"]?["xui"]?[0]?["xid"]?.ToString(),
                UserHash = job["DisplayClaims"]?["xui"]?[0]?["uhs"]?.ToString(),
                XSTSToken = job["Token"]?.ToString(),
                ExpireOn = job["NotAfter"]?.ToString()
            };
        }

        private JArray NullHandle(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            else
                return new JArray(str);
        }

        private JArray NullHandle(string[] strs)
        {
            if (strs == null || strs.Length == 0)
                return null;
            else
                return new JArray(strs);
        }
    }
}
