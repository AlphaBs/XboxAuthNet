using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using XboxAuthNet.XboxLive.Requests;

namespace XboxAuthNet.Test;

[TestFixture]
public class TestRequestBodySerialization
{
    [Test]
    public async Task UnsignedRpsOmitsNullProperties()
    {
        var body = new XboxUserRequestBody<RpsProperties>
        {
            RelyingParty = null,
            Properties = new RpsProperties
            {
                AuthMethod = null!,
                RpsTicket = null
            }
        };
        using var content = HttpHelper.CreateJsonContent(body);

        var json = await content.ReadAsStringAsync();

        Assert.That(json, Is.EqualTo("{\"TokenType\":\"JWT\",\"Properties\":{\"SiteName\":\"user.auth.xboxlive.com\"}}"));
    }

    [Test]
    public void SignedRpsPreservesNullPropertiesAndOrder()
    {
        var body = new SignedUserRequestBody
        {
            RelyingParty = null,
            Properties = new SignedRpsProperties
            {
                AuthMethod = null!,
                RpsTicket = null
            }
        };

        var json = JsonSerializer.Serialize(body, XboxJson.TypeInfo<SignedUserRequestBody>());

        Assert.That(json, Is.EqualTo("{\"RelyingParty\":null,\"TokenType\":\"JWT\",\"Properties\":{\"AuthMethod\":null,\"SiteName\":\"user.auth.xboxlive.com\",\"RpsTicket\":null}}"));
    }

    [Test]
    public async Task XstsOmitsNullOptionalProperties()
    {
        var body = new XboxUserRequestBody<XstsProperties>
        {
            RelyingParty = "party",
            Properties = new XstsProperties { UserTokens = new[] { "user" } }
        };
        using var content = HttpHelper.CreateJsonContent(body);

        var json = await content.ReadAsStringAsync();

        Assert.That(json, Is.EqualTo("{\"RelyingParty\":\"party\",\"TokenType\":\"JWT\",\"Properties\":{\"UserTokens\":[\"user\"],\"SandboxId\":\"RETAIL\"}}"));
    }

    [Test]
    public async Task XstsKeepsNonNullOptionalProperties()
    {
        var body = new XboxUserRequestBody<XstsProperties>
        {
            RelyingParty = "party",
            Properties = new XstsProperties
            {
                UserTokens = new[] { "user" },
                DeviceToken = "device",
                TitleToken = "title",
                OptionalDisplayClaims = new[] { "claim" }
            }
        };
        using var content = HttpHelper.CreateJsonContent(body);
        using var document = JsonDocument.Parse(await content.ReadAsStringAsync());
        var properties = document.RootElement.GetProperty("Properties");

        Assert.That(properties.GetProperty("DeviceToken").GetString(), Is.EqualTo("device"));
        Assert.That(properties.GetProperty("TitleToken").GetString(), Is.EqualTo("title"));
        Assert.That(properties.GetProperty("OptionalDisplayClaims")[0].GetString(), Is.EqualTo("claim"));
    }
}
