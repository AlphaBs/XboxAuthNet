using System.Text.Json.Serialization;

namespace XboxAuthNet.OAuth.CodeFlow.Parameters;

public class CodeFlowRefreshTokenParameter : CodeFlowParameter
{
    [JsonPropertyName("grant_type")]
    public string? GrantType { get; set; }

    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    protected override void AddQueries(Dictionary<string, string?> query)
    {
        base.AddQueries(query);

        AddQuery(query, "grant_type", GrantType);
        AddQuery(query, "client_secret", ClientSecret);
        AddQuery(query, "refresh_token", RefreshToken);
    }
}
