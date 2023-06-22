using System.Text.Json.Serialization;

namespace XboxAuthNet.OAuth.CodeFlow;

public class CodeFlowAccessTokenQuery : CodeFlowQuery
{
    public const string DefaultClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    [JsonPropertyName("grant_type")]
    public string? GrantType { get; set; }

    [JsonPropertyName("code_verifier")]
    public string? CodeVerifier { get; set; }

    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }

    [JsonPropertyName("client_assertion_type")]
    public string? ClientAssertionType { get; set; }

    [JsonPropertyName("client_assertion")]
    public string? CilentAssertion { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}