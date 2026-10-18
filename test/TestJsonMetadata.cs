using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.Test;

[TestFixture]
public class TestJsonMetadata
{
    [Test]
    public async Task CustomRequestUsesProvidedMetadata()
    {
        var payload = new JsonMetadataPayload { Value = "custom request" };
        using var content = HttpHelper.CreateJsonContent(payload, TestJsonContext.Default.JsonMetadataPayload);
        using var document = JsonDocument.Parse(await content.ReadAsStringAsync());

        Assert.That(document.RootElement.GetProperty("Value").GetString(), Is.EqualTo("custom request"));
    }

    [Test]
    public void CustomRequestWithoutMetadataIsRejected()
    {
        var payload = new JsonMetadataPayload { Value = "custom request" };

        Assert.Throws<NotSupportedException>(() => HttpHelper.CreateJsonContent(payload));
    }

    [Test]
    public async Task CustomResponseUsesProvidedMetadata()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"Value\":\"custom response\"}")
        };
        var handler = new XboxAuthResponseHandler();

        var payload = await handler.HandleResponse(response, TestJsonContext.Default.JsonMetadataPayload);

        Assert.That(payload.Value, Is.EqualTo("custom response"));
    }
}

internal sealed class JsonMetadataPayload
{
    public string? Value { get; set; }
}

[JsonSerializable(typeof(JsonMetadataPayload))]
internal partial class TestJsonContext : JsonSerializerContext
{
}
