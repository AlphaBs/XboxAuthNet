using XboxAuthNetConsole.Options;
using XboxAuthNetConsole.Cache;
using XboxAuthNet.XboxLive;
using XboxAuthNet.XboxLive.Requests;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNetConsole
{
    public class XboxAuthCommand : ICommand
    {
        private readonly XboxAuthClient _authClient;
        private readonly XboxAuthOptions _options;
        private readonly SessionCache _sessionCache;

        public XboxAuthCommand(
            XboxAuthClient authClient,
            XboxAuthOptions options,
            SessionCache sessionCache)
        {
            this._authClient = authClient;
            this._options = options;
            this._sessionCache = sessionCache;
        }

        XboxAuthResponse? userToken;
        XboxAuthResponse? deviceToken;
        XboxAuthResponse? titleToken;
        XboxAuthResponse? xstsToken;

        public async Task Execute(CancellationToken cancellationToken)
        {
            switch (_options.Mode)
            {
                case XboxAuthLoginMode.Basic:
                    await basicAuth();
                    printResponse();
                    break;
                case XboxAuthLoginMode.Full:
                    await fullAuth();
                    printResponse();
                    break;
                case XboxAuthLoginMode.Sisu:
                    var sisuResponse = await sisuAuth();
                    Program.Instance.Printer.Print(sisuResponse);
                    break;
                default:
                    return;
            }

            if (_options.Cache)
            {
                if (userToken != null)
                    _sessionCache.XboxUserToken = userToken;
                if (titleToken != null)
                    _sessionCache.XboxTitleToken = titleToken;
                if (deviceToken != null)
                    _sessionCache.XboxDeviceToken = deviceToken;
                if (xstsToken != null)
                    _sessionCache.XboxXstsToken = xstsToken;
            }
        }

        private async Task basicAuth()
        {
            userToken = await _authClient.RequestUserToken(_options.AccessToken);
            xstsToken = await _authClient.RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken.Token,
                RelyingParty = _options.RelyingParty,
                DeviceToken = deviceToken?.Token,
                TitleToken = null,
                OptionalDisplayClaims = null
            });
        }

        private async Task fullAuth()
        {
            userToken = await _authClient.RequestSignedUserToken(new XboxSignedUserTokenRequest
            {
                AccessToken = _options.AccessToken,
                TokenPrefix = _options.TokenPrefix
            });
            deviceToken = await _authClient.RequestDeviceToken(new XboxDeviceTokenRequest
            {
                DeviceType = _options.DeviceType,
                DeviceVersion = _options.DeviceVersion
            });
            //titleToken = await _authClient.RequestTitleToken(new XboxTitleTokenRequest
            //{
            //    AccessToken = accessToken,
            //    DeviceToken = deviceToken.Token
            //});
            xstsToken = await _authClient.RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken.Token,
                DeviceToken = deviceToken?.Token,
                TitleToken = titleToken?.Token,
                RelyingParty = _options.RelyingParty,
            });
        }

        private async Task<XboxSisuResponse> sisuAuth()
        {
            var result = await _authClient.SisuAuth(new XboxSisuAuthRequest
            {
                TokenPrefix = _options.TokenPrefix,
                AccessToken = _options.AccessToken,
                RelyingParty = _options.RelyingParty,
                ClientId = _options.ClientId,
                DeviceToken = deviceToken?.Token
            });

            userToken = result.UserToken;
            titleToken = result.TitleToken;
            deviceToken = new XboxAuthResponse
            {
                Token = result.DeviceToken
            };
            xstsToken = result.AuthorizationToken;
            return result;
        }

        private void printResponse()
        {
            Console.WriteLine("Xbox Auth Success");
            Console.WriteLine("UserToken: ");
            printResponse(userToken);
            Console.WriteLine("DeviceToken: ");
            printResponse(deviceToken);
            Console.WriteLine("TitleToken: ");
            printResponse(titleToken);
            Console.WriteLine("XstsToken: ");
            printResponse(xstsToken);
        }
    }
}