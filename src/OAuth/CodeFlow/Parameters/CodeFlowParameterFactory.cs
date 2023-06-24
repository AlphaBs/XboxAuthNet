namespace XboxAuthNet.OAuth.CodeFlow.Parameters;

public class CodeFlowParameterFactory
{
    public const string OAuthDesktop = "https://login.live.com/oauth20_desktop.srf";

    public CodeFlowAuthorizationParameter CreateAuthorizationParameter() => new ()
    {
        RedirectUri = OAuthDesktop,
        ResponseType = "code",
        ResponseMode = "query",
        Prompt = "select_account"
    };

    public CodeFlowAccessTokenParameter CreateAccessTokenParameter(string code) => new()
    {
        RedirectUrl = OAuthDesktop,
        GrantType = "authorization_code",
        Code = code,
    };

    public CodeFlowRefreshTokenParameter CreateRefreshTokenParameter(string refreshToken) => new()
    {
        GrantType = "refresh_token",
        RefreshToken = refreshToken,
    };
}