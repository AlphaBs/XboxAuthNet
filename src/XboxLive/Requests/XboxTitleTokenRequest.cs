using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Crypto;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests;

public class XboxTitleTokenRequest : AbstractXboxSignedAuthRequest
{
    public string? AccessToken { get; set; }
    public string? DeviceToken { get; set; }
    public string? TokenPrefix { get; set; } = XboxAuthConstants.XboxTokenPrefix;
    public string? RelyingParty { get; set; } = XboxAuthConstants.XboxAuthRelyingParty;

    protected override string RequestUrl => "https://title.auth.xboxlive.com/title/authenticate";
    protected override object BuildBody(object proofKey)
    {
        if (string.IsNullOrEmpty(AccessToken))
            throw new InvalidOperationException("AccessToken was null");
        if (string.IsNullOrEmpty(RelyingParty))
            throw new InvalidOperationException("RelyingParty was null");

        return new XboxTokenRequestBody<TitleProperties>
        {
            Properties = new TitleProperties
            {
                AuthMethod = "RPS",
                DeviceToken = DeviceToken,
                RpsTicket = TokenPrefix + AccessToken,
                SiteName = "user.auth.xboxlive.com",
                ProofKey = proofKey,
            },
            RelyingParty = RelyingParty,
            TokenType = "JWT"
        };
    }

    public Task<XboxAuthResponse> Send(HttpClient httpClient, IXboxRequestSigner signer)
    {
        return Send<XboxAuthResponse>(httpClient, signer);
    }
}

internal sealed class TitleProperties
{
    public string AuthMethod { get; set; } = "RPS";

    public string? DeviceToken { get; set; }

    public string? RpsTicket { get; set; }

    public string SiteName { get; set; } = "user.auth.xboxlive.com";

    public object? ProofKey { get; set; }
}
