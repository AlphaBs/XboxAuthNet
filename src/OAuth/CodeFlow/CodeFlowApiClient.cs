using System.Text.Json;

// https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow
// https://docs.microsoft.com/en-us/advertising/guides/authentication-oauth
// https://docs.microsoft.com/en-us/advertising/shopping-content/code-example-authentication-oauth

namespace XboxAuthNet.OAuth.CodeFlow;

public class CodeFlowLiveApiClient : ICodeFlowApiClient
{
    const string OAuthAuthorize = "https://login.live.com/oauth20_authorize.srf";
    const string OAuthErrorPath = "/err.srf";
    const string OAuthToken = "https://login.live.com/oauth20_token.srf";

    private readonly HttpClient httpClient;

    public CodeFlowLiveApiClient(string clientId, string scope, HttpClient client)
    {
        ClientId = clientId;
        Scope = scope;
        httpClient = client;
    }

    public string ClientId { get; }
    public string Scope { get; }

    public string CreateAuthorizeCodeUrl(CodeFlowAuthorizationQuery query)
    {
        if (string.IsNullOrEmpty(query.ClientId))
            query.ClientId = ClientId;
        if (string.IsNullOrEmpty(query.Scope))
            query.Scope = Scope;
        return OAuthAuthorize + "?" + HttpHelper.GetQueryString(query.ToQueryDictionary());
    }

    public string CreateSignoutUrl() =>
        "https://login.microsoftonline.com/consumer/oauth2/v2.0/logout";

    public async Task<MicrosoftOAuthResponse> RequestToken(
        CodeFlowQuery query, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(query.ClientId))
            query.ClientId = ClientId;
        if (string.IsNullOrEmpty(query.Scope))
            query.Scope = Scope;

        var queryDict = query.ToQueryDictionary();
        return await microsoftOAuthRequest(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(OAuthToken),
            Content = new FormUrlEncodedContent(queryDict!)
        }, cancellationToken).ConfigureAwait(false);
    }

    private async Task<MicrosoftOAuthResponse> microsoftOAuthRequest(
        HttpRequestMessage req,
        CancellationToken cancellationToken)
    {
        req.Headers.Add("User-Agent", HttpHelper.UserAgent);
        req.Headers.Add("Accept-Encoding", "gzip");
        req.Headers.Add("Accept-Language", "en-US");

        var res = await httpClient.SendAsync(req, cancellationToken)
            .ConfigureAwait(false);

        var resBody = await res.Content.ReadAsStringAsync()
            .ConfigureAwait(false);
        var statusCode = (int)res.StatusCode;
        var reasonPhrase = res.ReasonPhrase;
        return MicrosoftOAuthResponse.FromHttpResponse(resBody, statusCode, reasonPhrase);
    }
}
