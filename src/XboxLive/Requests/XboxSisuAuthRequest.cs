using System;
using System.Threading.Tasks;
using System.Net.Http;
using XboxAuthNet.XboxLive.Responses;
using XboxAuthNet.XboxLive.Crypto;

namespace XboxAuthNet.XboxLive.Requests;

public class XboxSisuAuthRequest : AbstractXboxSignedAuthRequest
{
    public string? TokenPrefix { get; set; } = XboxAuthConstants.XboxTokenPrefix;
    public string? AccessToken { get; set; }
    public string? RelyingParty { get; set; } = XboxAuthConstants.XboxLiveRelyingParty;
    public string? ClientId { get; set; }
    public string? DeviceToken { get; set; }

    protected override string RequestUrl => "https://sisu.xboxlive.com/authorize";
    protected override object BuildBody(object proofKey)
    {
        if (string.IsNullOrEmpty(AccessToken))
            throw new InvalidOperationException("AccessToken was null");
        if (string.IsNullOrEmpty(RelyingParty))
            throw new InvalidOperationException("RelyingParty was null");

        return new SisuRequestBody
        {
            AccessToken = TokenPrefix + AccessToken,
            AppId = ClientId,
            DeviceToken = DeviceToken,
            Sandbox = "RETAIL",
            UseModernGamertag = true,
            SiteName = "user.auth.xboxlive.com",
            RelyingParty = RelyingParty,
            ProofKey = proofKey
        };
    }

    public Task<XboxSisuResponse> Send(HttpClient httpClient, IXboxRequestSigner signer)
    {
        return Send<XboxSisuResponse>(httpClient, signer);
    }
}

internal sealed class SisuRequestBody
{
    public string? AccessToken { get; set; }

    public string? AppId { get; set; }

    public string? DeviceToken { get; set; }

    public string Sandbox { get; set; } = "RETAIL";

    public bool UseModernGamertag { get; set; } = true;

    public string SiteName { get; set; } = "user.auth.xboxlive.com";

    public string? RelyingParty { get; set; }

    public object? ProofKey { get; set; }
}
