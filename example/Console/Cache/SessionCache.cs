using XboxAuthNet.OAuth;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNetConsole.Cache
{
    public class SessionCache
    {
        public MicrosoftOAuthResponse? MicrosoftOAuth { get; set; }
        public XboxAuthResponse? XboxUserToken { get; set; }
        public XboxAuthResponse? XboxDeviceToken { get; set; }
        public XboxAuthResponse? XboxTitleToken { get; set; }
        public XboxAuthResponse? XboxXstsToken { get; set; }
    }
}