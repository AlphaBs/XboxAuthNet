using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive.Crypto;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests;

public class XboxSignedUserTokenRequest : AbstractXboxSignedAuthRequest
{
    public string? RelyingParty { get; set; } = XboxAuthConstants.XboxAuthRelyingParty;
    public string? TokenPrefix { get; set; } = XboxAuthConstants.XboxTokenPrefix;
    public string? AccessToken { get; set; }

    protected override string RequestUrl => "https://user.auth.xboxlive.com/user/authenticate";

    protected override object BuildBody(object proofKey)
    {
        if (string.IsNullOrEmpty(AccessToken))
            throw new InvalidOperationException("AccessToken was null");
        if (string.IsNullOrEmpty(RelyingParty))
            throw new InvalidOperationException("RelyingParty was null");

        return new SignedUserRequestBody
        {
            RelyingParty = RelyingParty,
            TokenType = "JWT",
            Properties = new SignedRpsProperties
            {
                AuthMethod = "RPS",
                SiteName = "user.auth.xboxlive.com",
                RpsTicket = TokenPrefix + AccessToken
            }
        };
    }

    public Task<XboxAuthResponse> Send(HttpClient httpClient, IXboxRequestSigner signer)
    {
        return Send<XboxAuthResponse>(httpClient, signer);
    }
}

internal sealed class SignedUserRequestBody
{
    public string? RelyingParty { get; set; }

    public string TokenType { get; set; } = "JWT";

    public SignedRpsProperties? Properties { get; set; }
}

internal sealed class SignedRpsProperties
{
    public string AuthMethod { get; set; } = "RPS";

    public string SiteName { get; set; } = "user.auth.xboxlive.com";

    public string? RpsTicket { get; set; }
}
