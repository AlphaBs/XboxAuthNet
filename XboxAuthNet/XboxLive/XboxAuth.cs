using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

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

        protected readonly HttpClient httpClient;

        public XboxAuth(HttpClient client)
        {
            this.httpClient = client;
        }

        private async Task<XboxAuthResponse> xboxRequest(HttpRequestMessage req, string contractVersion)
        {
            req.Headers.Add("User-Agent", HttpUtil.UserAgent);
            req.Headers.Add("Accept", "application/json");
            req.Headers.Add("Accept-Encoding", "gzip");
            req.Headers.Add("Accept-Language", "en-US");
            req.Headers.Add("x-xbl-contract-version", contractVersion);

            var res = await httpClient.SendAsync(req)
                .ConfigureAwait(false);
            return await handleXboxResponse(res);
        }

        internal async Task<XboxAuthResponse> handleXboxResponse(HttpResponseMessage res)
        {
            var resBody = await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            try
            {
                res.EnsureSuccessStatusCode();

                using var jsonDocument = JsonDocument.Parse(resBody);
                var root = jsonDocument.RootElement;
                var xboxResponse = root.Deserialize<XboxAuthResponse>();

                var xuis = root.GetProperty("DisplayClaims").GetProperty("xui").EnumerateArray();
                if (xboxResponse != null && xuis.Any())
                {
                    var xui = xuis.First();
                    xboxResponse.UserXUID = xui.GetProperty("xid").GetString();
                    xboxResponse.UserHash = xui.GetProperty("uhs").GetString();
                    return xboxResponse;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            catch (Exception ex) when (
                ex is JsonException ||
                ex is KeyNotFoundException ||
                ex is HttpRequestException)
            {
                try
                {
                    throw XboxAuthException.FromResponseBody(resBody, (int)res.StatusCode);
                }
                catch (FormatException)
                {
                    throw new XboxAuthException($"{(int)res.StatusCode}: {res.ReasonPhrase}", (int)res.StatusCode);
                }
            }
        }

        public async Task<XboxAuthResponse> ExchangeRpsTicketForUserToken(string rps)
        {
            var res = await xboxRequest(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(UserAuthenticate),
                Content = createJsonContent(new
                {
                    RelyingParty = "http://auth.xboxlive.com",
                    TokenType = "JWT",
                    Properties = new
                    {
                        AuthMethod = "RPS",
                        SiteName = "user.auth.xboxlive.com",
                        RpsTicket = rps
                    }
                })
            }, contractVersion: "0").ConfigureAwait(false);
            return res;
        }

        public async Task<XboxAuthResponse> ExchangeTokensForXstsIdentity(string userToken, string? deviceToken, string? titleToken,
            string? xstsRelyingParty, string[]? optionalDisplayClaims)
        {
            var res = await xboxRequest(new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(XstsAuthorize),
                Content = createJsonContent(new
                {
                    RelyingParty = xstsRelyingParty ?? DefaultRelyingParty,
                    TokenType = "JWT",
                    Properties = new
                    {
                        UserTokens = new string[] { userToken },
                        DeviceToken = deviceToken,
                        TitleToken = titleToken,
                        OptionalDisplayClaims = optionalDisplayClaims,
                        SandboxId = "RETAIL"
                    }
                }),
            }, contractVersion: "1").ConfigureAwait(false);
            return res;
        }

        private JsonContent createJsonContent<T>(T obj)
        {
            return JsonContent.Create(obj,
                mediaType: new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"),
                options: new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    PropertyNamingPolicy = null
                });
        }
    }
}
