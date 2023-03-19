namespace XboxAuthNet.OAuth.Models
{
    public class MicrosoftOAuthCodeCheckResult
    {
        public bool IsAuthResultFound { get; }
        public MicrosoftOAuthCode? OAuthCode { get; }

        public MicrosoftOAuthCodeCheckResult(bool result, MicrosoftOAuthCode code)
        {
            IsAuthResultFound = result;
            OAuthCode = code;
        }
    }
}