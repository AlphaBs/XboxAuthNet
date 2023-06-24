using CommandLine;

namespace XboxAuthNetConsole.Options
{
    public enum MicrosoftOAuthMode
    {
        Auto,
        Interactive,
        Silent,
        Signout,
        CreateUrl
    }

    [Verb("oauth")]
    public class MicrosoftOAuthOptions
    {
        [Option('m', "mode", Default=MicrosoftOAuthMode.Auto)]
        public MicrosoftOAuthMode Mode { get; set; } = MicrosoftOAuthMode.Auto;

        [Option("cache", Default=true)]
        public bool Cache { get; set; } = true;

        [Option("clientId")]
        public string? ClientId { get; set; }

        [Option("scopes")]
        public string? Scopes { get; set; }

        [Option('t', "accessToken")]
        public string? AccessToken { get; set; }

        [Option("refreshToken")]
        public string? RefreshToken { get; set; }

        [Option("propmt")]
        public string? Prompt { get; set; }

        [Option("loginHint")]
        public string? LoginHint { get; set; }
    }
}