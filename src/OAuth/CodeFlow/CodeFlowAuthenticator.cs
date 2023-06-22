namespace XboxAuthNet.OAuth.CodeFlow;

public class CodeFlowAuthenticator
{
    private readonly ICodeFlowApiClient _client;
    private readonly ICodeFlowUrlChecker _uriChecker;
    private readonly IWebUI _ui;
    private readonly CodeFlowQueryFactory _queryFactory;

    internal CodeFlowAuthenticator(
        ICodeFlowApiClient client,
        IWebUI ui,
        ICodeFlowUrlChecker urlChecker)
    {
        _client = client;
        _ui = ui;
        _uriChecker = urlChecker;
        _queryFactory = new CodeFlowQueryFactory();
    }

    public Task<MicrosoftOAuthResponse> AuthenticateInteractively(CancellationToken cancellationToken = default)
        => AuthenticateInteractively(
            _ => {},
            _ => {},
            cancellationToken);

    public async Task<MicrosoftOAuthResponse> AuthenticateInteractively(
        Action<CodeFlowAuthorizationQuery> codeQueryInvoker,
        Action<CodeFlowAccessTokenQuery> tokenQueryInvoker,
        CancellationToken cancellationToken = default)
    {
        var codeQuery = _queryFactory.CreateAuthorizeCodeQuery();
        codeQueryInvoker(codeQuery);

        var tokenQuery = _queryFactory.CreateAccessTokenQuery();
        tokenQueryInvoker(tokenQuery);

        var uri = _client.CreateAuthorizeCodeUrl(codeQuery);
        var authCode = await _ui.DisplayDialogAndInterceptUri(
            new Uri(uri), _uriChecker, cancellationToken);

        if (!authCode.IsSuccess)
        {
            throw new AuthCodeException(authCode.Error, authCode.ErrorDescription);
        }

        if (string.IsNullOrEmpty(tokenQuery.Code))
            tokenQuery.Code = authCode.Code;

        return await _client.RequestToken(
            tokenQuery, 
            cancellationToken);
    }

    public Task<MicrosoftOAuthResponse> AuthenticateSilently(
        string refreshToken,
        CancellationToken cancellationToken = default) =>
        AuthenticateSilently(refreshToken, _ => {}, cancellationToken);

    public Task<MicrosoftOAuthResponse> AuthenticateSilently(
        string refreshToken,
        Action<CodeFlowRefreshTokenQuery> queryInvoker,
        CancellationToken cancellationToken = default)
    {
        var query = _queryFactory.CreateRefreshTokenQuery(refreshToken);
        queryInvoker(query);
        return _client.RequestToken(query, cancellationToken);
    }

    public async Task Signout(CancellationToken cancellationToken = default)
    {
        var url = _client.CreateSignoutUrl();
        await _ui.DisplayDialogAndNavigateUri(new Uri(url), cancellationToken);
    }
}
