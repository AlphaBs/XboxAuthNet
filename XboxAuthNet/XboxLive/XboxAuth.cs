﻿using Microsoft.Extensions.Logging;
using System;
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

        private readonly HttpClient httpClient;
        private readonly ILogger<XboxAuth>? logger;

        public XboxAuth(HttpClient client, ILoggerFactory? logFactory = null)
        {
            this.httpClient = client;
            this.logger = logFactory?.CreateLogger<XboxAuth>();
        }

        private async Task<XboxAuthResponse> xboxRequest(HttpRequestMessage req, string contractVersion)
        {
            req.Headers.Add("User-Agent", HttpUtil.UserAgent);
            req.Headers.Add("Accept", "application/json");
            req.Headers.Add("Accept-Encoding", "gzip");
            req.Headers.Add("Accept-Language", "en-US");
            req.Headers.Add("x-xbl-contract-version", contractVersion);

            logger?.LogTrace("Request to {RequestUri}", req.RequestUri.ToString());

            var res = await httpClient.SendAsync(req)
                .ConfigureAwait(false);
            var resBody = await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            logger?.LogTrace("code: {Code}, body: {Body}", res.StatusCode, resBody);

            res.EnsureSuccessStatusCode();
            return parseAuthResponse(resBody);
        }

        private XboxAuthResponse parseAuthResponse(string json)
        {
            using var jsonDocument = JsonDocument.Parse(json);
            var root = jsonDocument.RootElement;
            var xboxResponse = root.Deserialize<XboxAuthResponse>();

            if (xboxResponse != null &&
                root.TryGetProperty("DisplayClaims", out var displayClaims) &&
                displayClaims.TryGetProperty("xui", out var xui))
            {
                var xuis = xui.EnumerateArray();
                if (xuis.Any())
                {
                    var firstXui = xuis.First();
                    if (firstXui.TryGetProperty("xid", out var xid))
                        xboxResponse.UserXUID = xid.GetString();

                    if (firstXui.TryGetProperty("uhs", out var uhs))
                        xboxResponse.UserHash = uhs.GetString();
                }
            }

            return xboxResponse ?? new XboxAuthResponse(false);
        }

        public async Task<XboxAuthResponse> ExchangeRpsTicketForUserToken(string rps)
        {
            try
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
            catch (Exception ex)
            {
                throw new XboxAuthException("Failed to " + nameof(ExchangeRpsTicketForUserToken), null, ex);
            }
        }

        public async Task<XboxAuthResponse> ExchangeTokensForXstsIdentity(string userToken, string? deviceToken, string? titleToken,
            string? xstsRelyingParty, string[]? optionalDisplayClaims)
        {
            try
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
            catch (Exception ex)
            {
                throw new XboxAuthException("Failed to" + nameof(ExchangeTokensForXstsIdentity), null, ex);
            }
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
