using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using XboxAuthNet;
using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.CodeFlow.Parameters;
using XboxAuthNet.XboxLive;
using XboxAuthNet.XboxLive.Crypto;
using XboxAuthNet.XboxLive.Requests;
using XboxAuthNet.XboxLive.Responses;

var checks = 0;
const string responseBody = "{\"Token\":\"fixture-token\",\"DisplayClaims\":{\"xui\":[{\"uhs\":\"hash\",\"xid\":\"123\"}]}}";
var handler = new FixtureHandler(responseBody);
using var http = new HttpClient(handler);
var signer = new FixtureSigner();

var user = await new XboxUserTokenRequest { AccessToken = "fixture" }.Send(http);
Check(user.Token == "fixture-token" && user.XuiClaims?.UserHash == "hash", "user response");
Check(handler.Body == "{\"RelyingParty\":\"http://auth.xboxlive.com\",\"TokenType\":\"JWT\",\"Properties\":{\"AuthMethod\":\"RPS\",\"SiteName\":\"user.auth.xboxlive.com\",\"RpsTicket\":\"fixture\"}}", "exact user body");

await new XboxXstsRequest { UserToken = "user" }.Send(http);
using (var json = JsonDocument.Parse(handler.Body))
{
    var properties = json.RootElement.GetProperty("Properties");
    Check(properties.GetProperty("UserTokens")[0].GetString() == "user", "xsts token");
    Check(!properties.TryGetProperty("DeviceToken", out _), "xsts null omitted");
}

await new XboxSignedUserTokenRequest { AccessToken = "user" }.Send(http, signer);
Check(handler.Body == signer.SignedBody && handler.HasSignature, "signed user bytes");
await new XboxDeviceTokenRequest
{
    Id = "id",
    SerialNumber = "serial",
    DeviceType = "Win32",
    DeviceVersion = "10"
}.Send(http, signer);
using (var json = JsonDocument.Parse(handler.Body))
{
    var properties = json.RootElement.GetProperty("Properties");
    Check(properties.GetProperty("ProofKey").GetProperty("kty").GetString() == "EC", "device proof key");
    Check(properties.GetProperty("Id").GetString() == "{id}", "device id braces");
}
Check(handler.Body == signer.SignedBody, "device signature bytes");
await new XboxTitleTokenRequest { AccessToken = "title" }.Send(http, signer);
using (var json = JsonDocument.Parse(handler.Body))
{
    Check(json.RootElement.GetProperty("Properties").GetProperty("DeviceToken").ValueKind == JsonValueKind.Null, "signed null retained");
}
Check(handler.Body == signer.SignedBody, "title signature bytes");
handler.Response = "{\"DeviceToken\":\"device\",\"UserToken\":" + responseBody + "}";
var sisu = await new XboxSisuAuthRequest { AccessToken = "sisu", ClientId = "app" }.Send(http, signer);
Check(sisu.UserToken?.XuiClaims?.XboxUserId == "123", "nested sisu response");
Check(handler.Body == signer.SignedBody, "sisu signature bytes");

foreach (var xui in new[] { "[{\"uhs\":\"first\"},{\"uhs\":\"second\"}]", "{\"uhs\":\"first\"}" })
{
    using var response = Response("{\"DisplayClaims\":{\"unknown\":{},\"xui\":" + xui + "}}");
    var parsed = await new XboxAuthResponseHandler().HandleResponse<XboxAuthResponse>(response);
    Check(parsed.XuiClaims?.UserHash == "first", "xui array/object");
}

using (var response = Response("{\"XErr\":2148916233,\"Message\":\"denied\"}", HttpStatusCode.Unauthorized))
{
    try
    {
        await new XboxAuthResponseHandler().HandleResponse<XboxAuthResponse>(response);
        throw new Exception("Expected Xbox error");
    }
    catch (XboxAuthException ex)
    {
        Check(ex.StatusCode == 401 && ex.Error == "8015dc09", "numeric Xbox error");
    }
}
using (var response = Response("invalid", HttpStatusCode.BadRequest))
{
    response.Headers.Add("X-Err", "2148916233");
    try
    {
        await new XboxAuthResponseHandler().HandleResponse<XboxAuthResponse>(response);
        throw new Exception("Expected header error");
    }
    catch (XboxAuthException ex)
    {
        Check(ex.Error == "8015dc09", "header fallback");
    }
}

var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"name\":\"fixture\",\"roles\":[\"player\"]}"))
    .TrimEnd('=')
    .Replace('+', '-')
    .Replace('/', '_');
var oauth = MicrosoftOAuthResponse.FromHttpResponse("{\"access_token\":\"access\",\"expires_in\":3600,\"refresh_token\":\"M.R3_BAY.refresh\",\"id_token\":\"a." + payload + ".b\"}", 200, null);
Check(oauth.DecodeIdTokenPayload()?.Name == "fixture", "JWT payload");
Check(oauth.RefreshToken == "refresh" && oauth.ExpiresOn > DateTimeOffset.UtcNow, "OAuth metadata");

var authorization = new CodeFlowAuthorizationParameter
{
    ClientId = "client",
    Tenant = "tenant",
    Scope = "scope",
    ResponseType = "code",
    RedirectUri = "https://localhost/callback",
    Nonce = "nonce",
    State = ""
};
authorization.ExtraQueries["scope"] = "override";
var query = authorization.ToQueryDictionary();
Check(query["nonce"] == "nonce" && query["scope"] == "override", "authorization mapping and override");
Check(!query.ContainsKey("Tenant") && !query.ContainsKey("state"), "non-query/empty excluded");
var access = new CodeFlowAccessTokenParameter { Code = "code", CilentAssertion = "assertion", CodeVerifier = "verifier" }.ToQueryDictionary();
Check(access["client_assertion"] == "assertion" && access["code_verifier"] == "verifier", "access mapping");
var refresh = new CodeFlowRefreshTokenParameter { RefreshToken = "refresh", GrantType = "refresh_token" }.ToQueryDictionary();
Check(refresh["refresh_token"] == "refresh", "refresh mapping");
var custom = new CustomParameter { Value = "custom" }.ToQueryDictionary();
Check(custom["custom"] == "custom", "custom query extension");

using (var content = HttpHelper.CreateJsonContent(new CustomResponse { Value = "body" }, SmokeJsonContext.Default.CustomResponse))
{
    Check((await content.ReadAsStringAsync()).Contains("body"), "custom body metadata");
}
using (var response = Response("{\"Value\":\"response\"}"))
{
    var parsed = await new XboxAuthResponseHandler().HandleResponse(response, SmokeJsonContext.Default.CustomResponse);
    Check(parsed.Value == "response", "custom response metadata");
}
var claims = new XboxAuthResponse { XuiClaims = new XboxAuthXuiClaims { UserHash = "roundtrip" } };
var claimsJson = JsonSerializer.Serialize(claims, SmokeJsonContext.Default.XboxAuthResponse);
var restoredClaims = JsonSerializer.Deserialize(claimsJson, SmokeJsonContext.Default.XboxAuthResponse);
Check(restoredClaims?.XuiClaims?.UserHash == "roundtrip", "claims write/read metadata");
handler.Response = "{\"Value\":\"custom-signed-response\"}";
var customSigned = await new CustomSignedRequest().Send(http, signer, SmokeJsonContext.Default.CustomResponse);
Check(customSigned.Value == "custom-signed-response", "custom signed response metadata");
Check(handler.Body == "{\"Value\":\"custom-signed-body\"}" && handler.Body == signer.SignedBody, "custom signed body metadata");

// JIT 이용자의 기존 익명 모델 경로도 별도 실행에서 검증한다.
if (JsonSerializer.IsReflectionEnabledByDefault)
{
    using var content = HttpHelper.CreateJsonContent(new { Value = "legacy" });
    Check((await content.ReadAsStringAsync()).Contains("legacy"), "legacy JIT model");
}
Console.WriteLine($"Passed {checks} XboxAuthNet protocol checks. JSON reflection: {JsonSerializer.IsReflectionEnabledByDefault}");

void Check(bool condition, string name)
{
    if (!condition)
        throw new Exception(name);

    checks++;
}

static HttpResponseMessage Response(string body, HttpStatusCode status = HttpStatusCode.OK)
{
    return new HttpResponseMessage(status) { Content = new StringContent(body) };
}

sealed class FixtureHandler(string response) : HttpMessageHandler
{
    public string Response { get; set; } = response;
    public string Body { get; private set; } = "";
    public bool HasSignature { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        Body = await request.Content!.ReadAsStringAsync(token);
        HasSignature = request.Headers.Contains("Signature");
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Response) };
    }
}

sealed class FixtureSigner : IXboxRequestSigner
{
    private readonly ECDCertificatePopCryptoProvider provider = new();
    public object ProofKey => provider.ProofKey;
    public string SignedBody { get; private set; } = "";

    public string SignRequest(string url, string token, string body)
    {
        SignedBody = body;
        return "fixture-signature";
    }
}

sealed class CustomParameter : CodeFlowParameter
{
    [JsonPropertyName("custom")]
    public string? Value { get; set; }

    protected override void AddQueries(Dictionary<string, string?> query)
    {
        base.AddQueries(query);
        query["custom"] = Value;
    }
}

public sealed class CustomResponse
{
    public string? Value { get; set; }
}

sealed class CustomSignedRequest : AbstractXboxSignedAuthRequest
{
    protected override string RequestUrl => "https://fixture.invalid/custom";

    protected override object BuildBody(object proofKey)
    {
        return new CustomResponse { Value = "custom-signed-body" };
    }

    protected override string SerializeBody(object body)
    {
        return JsonSerializer.Serialize((CustomResponse)body, SmokeJsonContext.Default.CustomResponse);
    }
}

[JsonSerializable(typeof(XboxAuthResponse))]
[JsonSerializable(typeof(XboxAuthXuiClaims))]
[JsonSerializable(typeof(CustomResponse))]
internal partial class SmokeJsonContext : JsonSerializerContext
{
}
