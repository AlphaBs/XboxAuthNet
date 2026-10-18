using System.Text.Json.Serialization;

namespace XboxAuthNet;

// 속성 순서와 null 처리 방식은 서명에 사용되는 기존 요청 본문과 동일하게 유지한다.
internal sealed class XboxTokenRequestBody<T>
{
    public T? Properties { get; set; }
    public string? RelyingParty { get; set; }
    public string TokenType { get; set; } = "JWT";
}

internal sealed class XboxUserRequestBody<T>
{
    public string? RelyingParty { get; set; }
    public string TokenType { get; set; } = "JWT";
    public T? Properties { get; set; }
}

internal sealed class RpsProperties
{
    public string AuthMethod { get; set; } = "RPS";
    public string SiteName { get; set; } = "user.auth.xboxlive.com";
    public string? RpsTicket { get; set; }
}

internal sealed class XstsProperties
{
    public string[]? UserTokens { get; set; }
    public string? DeviceToken { get; set; }
    public string? TitleToken { get; set; }
    public string[]? OptionalDisplayClaims { get; set; }
    public string SandboxId { get; set; } = "RETAIL";
}

internal sealed class DeviceProperties
{
    public string AuthMethod { get; set; } = "ProofOfPossession";
    public string? Id { get; set; }
    public string? DeviceType { get; set; }
    public string? SerialNumber { get; set; }
    public string? Version { get; set; }
    public object? ProofKey { get; set; }
}

internal sealed class TitleProperties
{
    public string AuthMethod { get; set; } = "RPS";
    public string? DeviceToken { get; set; }
    public string? RpsTicket { get; set; }
    public string SiteName { get; set; } = "user.auth.xboxlive.com";
    public object? ProofKey { get; set; }
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

internal sealed class ProofKeyBody
{
    [JsonPropertyName("kty")]
    public string KeyType { get; set; } = "EC";
    [JsonPropertyName("x")]
    public string? X { get; set; }
    [JsonPropertyName("y")]
    public string? Y { get; set; }
    [JsonPropertyName("crv")]
    public string Curve { get; set; } = "P-256";
    [JsonPropertyName("alg")]
    public string Algorithm { get; set; } = "ES256";
    [JsonPropertyName("use")]
    public string Use { get; set; } = "sig";
}
