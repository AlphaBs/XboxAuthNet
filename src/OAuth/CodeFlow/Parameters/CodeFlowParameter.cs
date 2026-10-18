using System.Text.Json.Serialization;

namespace XboxAuthNet.OAuth.CodeFlow.Parameters;

public class CodeFlowParameter
{
    public string? Tenant { get; set; }

    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    public Dictionary<string, string> ExtraQueries { get; } = new();

    public Dictionary<string, string?> ToQueryDictionary()
    {
        var query = new Dictionary<string, string?>();
        AddQueries(query);
        foreach (var pair in ExtraQueries)
        {
            query[pair.Key] = pair.Value;
        }

        return query;
    }

    /// <summary>사용자 정의 매개변수를 추가하려면 이 메서드를 재정의한다.</summary>
    protected virtual void AddQueries(Dictionary<string, string?> query)
    {
        AddQuery(query, "client_id", ClientId);
        AddQuery(query, "scope", Scope);
    }

    protected static void AddQuery(Dictionary<string, string?> query, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            query[name] = value;
    }

}
