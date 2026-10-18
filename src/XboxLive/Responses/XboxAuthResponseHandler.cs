using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace XboxAuthNet.XboxLive.Responses;

public class XboxAuthResponseHandler
{
    public Task<T> HandleResponse<T>(HttpResponseMessage res)
    {
        return HandleResponse(res, XboxJson.TypeInfo<T>());
    }

    public async Task<T> HandleResponse<T>(HttpResponseMessage res, JsonTypeInfo<T> typeInfo)
    {
        var resBody = await res.Content.ReadAsStringAsync();

        try
        {
            res.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize(resBody, typeInfo)
                ?? throw new JsonException();
        }
        catch (Exception ex) when (
            ex is JsonException ||
            ex is HttpRequestException)
        {
            try
            {
                throw XboxAuthException.FromResponseBody(resBody, (int)res.StatusCode);
            }
            catch (FormatException)
            {
                try
                {
                    throw XboxAuthException.FromResponseHeaders(res.Headers, (int)res.StatusCode);
                }
                catch (FormatException)
                {
                    throw new XboxAuthException($"{(int)res.StatusCode}: {res.ReasonPhrase}", (int)res.StatusCode);
                }
            }
        }
    }
}
