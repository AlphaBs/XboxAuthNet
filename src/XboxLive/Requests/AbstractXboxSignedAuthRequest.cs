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

    /// <summary>사용자 정의 응답의 생성 메타데이터를 지정한다.</summary>
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
        var bodyStr = SerializeBody(body);

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

    /// <summary>사용자 정의 요청은 생성 JsonTypeInfo를 사용해 이 메서드를 재정의할 수 있다.</summary>
    protected virtual string SerializeBody(object body)
    {
        return XboxJson.Serialize(body);
    }

    protected abstract object BuildBody(object proofKey);
}