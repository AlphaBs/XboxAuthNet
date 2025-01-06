using CommandLine;
using XboxAuthNet.XboxLive;

namespace XboxAuthNetConsole.Options
{
    public enum XboxAuthLoginMode
    {
        Basic,
        Full,
        Sisu,
        Validate
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

        [Option("tokenPrefix", Default=XboxAuthConstants.XboxTokenPrefix)]
        public string? TokenPrefix { get; set; }

        [Option("relyingParty", Default=XboxAuthConstants.XboxLiveRelyingParty)]
        public string? RelyingParty { get; set; }

        [Option("clientId")]
        public string? ClientId { get; set; }

        [Option("deviceType", Default=XboxDeviceTypes.Nintendo)]
        public string? DeviceType { get; set; }

        [Option("deviceVersion", Default="0.0.0")]
        public string? DeviceVersion { get; set; }
    }   
}