using System.Text.Json.Serialization;

namespace XboxAuthNet.OAuth.CodeFlow.Parameters;

public class CodeFlowAccessTokenParameter : CodeFlowParameter
{
    public const string DefaultClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";


    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("redirect_uri")]
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

    protected override void AddQueries(Dictionary<string, string?> query)
    {
        base.AddQueries(query);

        AddQuery(query, "code", Code);
        AddQuery(query, "redirect_uri", RedirectUrl);
        AddQuery(query, "grant_type", GrantType);
        AddQuery(query, "code_verifier", CodeVerifier);
        AddQuery(query, "client_secret", ClientSecret);
        AddQuery(query, "client_assertion_type", ClientAssertionType);
        AddQuery(query, "client_assertion", CilentAssertion);
    }
}