using System.Text.Json.Serialization.Metadata;
using System.Text;
using System.Text.Json;
using XboxAuthNet.XboxLive.Crypto;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests;

public abstract class AbstractXboxSignedAuthRequest
{
    public XboxAuthResponseHandler ResponseHandler { get; set; } = new();
    protected abstract string RequestUrl { get; }
    protected virtual string Token { get; } = "";

    public Task<T> Send<T>(HttpClient httpClient, IXboxRequestSigner signer)
    {
        return Send(httpClient, signer, XboxJson.TypeInfo<T>());
    }

    public async Task<T> Send<T>(HttpClient httpClient, IXboxRequestSigner signer, JsonTypeInfo<T> responseTypeInfo)
    {
        if (ResponseHandler == null)
            throw new InvalidOperationException("ResponseHandler was null");

        var request = buildRequest(signer);
        var response = await httpClient.SendAsync(request);
        return await ResponseHandler.HandleResponse(response, responseTypeInfo);
    }

    private HttpRequestMessage buildRequest(IXboxRequestSigner signer)
    {
        var body = BuildBody(signer.ProofKey);
        var bodyStr = JsonSerializer.Serialize(body, XboxJson.Options.GetTypeInfo(body.GetType()));

        var req = new HttpRequestMessage
        {
            RequestUri = new Uri(RequestUrl),
            Method = HttpMethod.Post,
            Content = new StringContent(bodyStr, Encoding.UTF8, "application/json")
        };

        var signature = signer.SignRequest(RequestUrl, Token, bodyStr);
        req.Headers.Add("Signature", signature);
        CommonRequestHeaders.AddDefaultHeaders(req);
        return req;
    }

    protected abstract object BuildBody(object proofKey);
}
