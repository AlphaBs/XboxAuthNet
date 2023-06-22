namespace XboxAuthNet.OAuth.CodeFlow;

public interface ICodeFlowApiClient
{
    Task<MicrosoftOAuthResponse> RequestToken(
        CodeFlowQuery query,
        CancellationToken cancellationToken);

    string CreateAuthorizeCodeUrl(CodeFlowAuthorizationQuery query);
    string CreateSignoutUrl();
}