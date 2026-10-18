using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using XboxAuthNet.OAuth;
using XboxAuthNet.XboxLive.Crypto;
using XboxAuthNet.XboxLive.Requests.Bodies;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(MicrosoftOAuthResponse))]
[JsonSerializable(typeof(MicrosoftUserPayload))]
[JsonSerializable(typeof(XboxAuthResponse))]
[JsonSerializable(typeof(XboxSisuResponse))]
[JsonSerializable(typeof(XboxErrorResponse))]
[JsonSerializable(typeof(XboxAuthXuiClaims))]
[JsonSerializable(typeof(XboxTokenRequestBody<DeviceProperties>))]
[JsonSerializable(typeof(XboxTokenRequestBody<TitleProperties>))]
[JsonSerializable(typeof(XboxUserRequestBody<RpsProperties>))]
[JsonSerializable(typeof(XboxUserRequestBody<XstsProperties>))]
[JsonSerializable(typeof(SisuRequestBody))]
[JsonSerializable(typeof(ProofKeyBody))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(JsonObject))]
internal partial class XboxJsonContext : JsonSerializerContext
{
}
