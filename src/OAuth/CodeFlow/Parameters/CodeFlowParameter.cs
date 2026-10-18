using System.Text.Json.Serialization;
using System.Text.Json;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

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
        // JIT에서는 기존 상속 모델의 attribute 기반 확장 동작을 유지한다.
        if (XboxJson.ReflectionEnabled)
            query = ToReflectionQueryDictionary();

        AddQueries(query);
        foreach (var pair in ExtraQueries)
        {
            query[pair.Key] = pair.Value;
        }

        return query;
    }

    /// <summary>AOT에서 사용자 정의 매개변수를 추가하려면 이 메서드를 재정의한다.</summary>
    protected virtual void AddQueries(Dictionary<string, string?> query)
    {
        AddQuery(query, "client_id", ClientId);
        AddQuery(query, "scope", Scope);

        if (this is CodeFlowAuthorizationParameter authorization)
        {
            AddQuery(query, "response_type", authorization.ResponseType);
            AddQuery(query, "redirect_uri", authorization.RedirectUri);
            AddQuery(query, "response_mode", authorization.ResponseMode);
            AddQuery(query, "state", authorization.State);
            AddQuery(query, "prompt", authorization.Prompt);
            AddQuery(query, "login_hint", authorization.LoginHint);
            AddQuery(query, "domain_hint", authorization.DomainHint);
            AddQuery(query, "code_challenge", authorization.CodeChallenge);
            AddQuery(query, "code_challenge_method", authorization.CodeChallengeMethod);
            AddQuery(query, "nonce", authorization.Nonce);
        }

        if (this is CodeFlowAccessTokenParameter access)
        {
            AddQuery(query, "code", access.Code);
            AddQuery(query, "redirect_uri", access.RedirectUrl);
            AddQuery(query, "grant_type", access.GrantType);
            AddQuery(query, "code_verifier", access.CodeVerifier);
            AddQuery(query, "client_secret", access.ClientSecret);
            AddQuery(query, "client_assertion_type", access.ClientAssertionType);
            AddQuery(query, "client_assertion", access.CilentAssertion);
        }

        if (this is CodeFlowRefreshTokenParameter refresh)
        {
            AddQuery(query, "grant_type", refresh.GrantType);
            AddQuery(query, "client_secret", refresh.ClientSecret);
            AddQuery(query, "refresh_token", refresh.RefreshToken);
        }
    }

    private static void AddQuery(Dictionary<string, string?> query, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            query[name] = value;
    }

#if NET8_0_OR_GREATER
    [RequiresUnreferencedCode("사용자 정의 쿼리 모델의 속성은 트리밍될 수 있습니다. AOT에서는 AddQueries를 재정의하세요.")]
#endif
    private Dictionary<string, string?> ToReflectionQueryDictionary()
    {
        var query = new Dictionary<string, string?>();
        var props = GetType().GetProperties();
        foreach (var prop in props)
        {
            var value = prop.GetMethod?.Invoke(this, null)?.ToString();
            if (string.IsNullOrEmpty(value))
                continue;

            var attr = prop
                .GetCustomAttributes(typeof(JsonPropertyNameAttribute), true)
                .FirstOrDefault();
            if (attr is not JsonPropertyNameAttribute jsonAttr)
                continue;

            var propName = jsonAttr.Name;
            query[propName] = value;
        }

        return query;
    }
}
