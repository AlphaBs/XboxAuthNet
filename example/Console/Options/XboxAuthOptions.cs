using CommandLine;

namespace XboxAuthNetConsole.Options
{
    public enum XboxAuthLoginMode
    {
        Basic,
        Full,
        Sisu
    }

    [Verb("xboxauth")]
    public class XboxAuthOptions
    {
        [Option('m', "mode", Default=XboxAuthLoginMode.Basic)]
        public XboxAuthLoginMode Mode { get; set; }

        [Option("cache", Default=true)]
        public bool Cache { get; set; } = true;

        [Option('t', "accessToken")]
        public string? AccessToken { get; set; }

        public string? TokenPrefix { get; set; }

        public string? RelyingParty { get; set; }

        public string? ClientId { get; set; }

        public string? DeviceType { get; set; }

        public string? DeviceVersion { get; set; }
    }   
}