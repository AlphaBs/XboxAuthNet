using System.Text.Json.Serialization.Metadata;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.XboxLive.Requests;

public abstract class AbstractXboxAuthRequest
{
    public XboxAuthResponseHandler? ResponseHandler { get; set; } = new();
    public string? ContractVersion { get; set; }

    protected abstract HttpRequestMessage BuildRequest();

    public Task<T> Send<T>(HttpClient httpClient)
    {
        return Send(httpClient, XboxJson.TypeInfo<T>());
    }

    /// <summary>사용자 정의 응답의 생성 메타데이터를 지정한다.</summary>
    public async Task<T> Send<T>(HttpClient httpClient, JsonTypeInfo<T> responseTypeInfo)
    {
        if (ResponseHandler == null)
            throw new InvalidOperationException("ResponseHandler was null");

        var request = BuildRequest();
        request.Headers.Add("x-xbl-contract-version", ContractVersion ?? "");

        var response = await httpClient.SendAsync(request);
        return await ResponseHandler.HandleResponse(response, responseTypeInfo);
    }
}