namespace XboxAuthNet.OAuth.CodeFlow;

public class CodeFlowQueryFactory
{
    const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";

    public CodeFlowAuthorizationQuery CreateAuthorizeCodeQuery() => new ()
    {
        ResponseType = "code",
        ResponseMode = "query",
        Prompt = "select_account"
    };

    public CodeFlowAccessTokenQuery CreateAccessTokenQuery() => new()
    {
        RedirectUrl = OAuthDesktop,
        GrantType = "authorization_code",
    };

    public CodeFlowAccessTokenQuery CreateAccessTokenQuery(string code) => new()
    {
        RedirectUrl = OAuthDesktop,
        GrantType = "authorization_code",
        Code = code,
    };

    public CodeFlowRefreshTokenQuery CreateRefreshTokenQuery(string refreshToken) => new()
    {
        GrantType = "refresh_token",
        RefreshToken = refreshToken,
    };

    public CodeFlowRefreshTokenQuery CreateRefreshTokenQuery(string refreshToken, string clientSecret) => new()
    {
        GrantType = "refresh_token",
        RefreshToken = refreshToken,
        ClientSecret = clientSecret
    };
}