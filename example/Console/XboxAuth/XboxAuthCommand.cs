using System.Diagnostics;
using XboxAuthNetConsole.Options;
using XboxAuthNetConsole.Cache;
using XboxAuthNetConsole.Printer;
using XboxAuthNet.XboxLive;
using XboxAuthNet.XboxLive.Requests;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNetConsole
{
    public class XboxAuthCommand : ICommand
    {
        private readonly XboxAuthOptions _options;
        private readonly SessionCache _sessionCache;

        public XboxAuthCommand(
            XboxAuthOptions options,
            SessionCache sessionCache)
        {
            this._options = options;
            this._sessionCache = sessionCache;
        }

        XboxAuthResponse? userToken;
        XboxAuthResponse? deviceToken;
        XboxAuthResponse? titleToken;
        XboxAuthResponse? xstsToken;

        public async Task Execute(CancellationToken cancellationToken)
        {
            if (_options == null)
                throw new InvalidOperationException("_options was null");

            loadSessionCaches();

            _options.AccessToken = _sessionCache.MicrosoftOAuth?.AccessToken;
            if (string.IsNullOrEmpty(_options.AccessToken))
                throw new InvalidOperationException(
                    "No Microsoft OAuth access token was specified.\n" +
                    "Specify '--accessToken' or run Microsoft OAuth first");

            var authClient = initializeAuthClient();
            var signedClient = initializeSignedClient();
            switch (_options.Mode)
            {
                case XboxAuthLoginMode.Basic:
                    await basicAuth(authClient);
                    printResponse();
                    break;
                case XboxAuthLoginMode.Full:
                    await fullAuth(authClient, signedClient);
                    printResponse();
                    break;
                case XboxAuthLoginMode.Sisu:
                    var sisuResponse = await sisuAuth(signedClient);
                    ConsolePrinter.Print(sisuResponse);
                    break;
                case XboxAuthLoginMode.Validate:
                    var result = xstsToken != null && xstsToken.Validate();
                    Console.WriteLine("XSTS: " + result);
                    break;
                default:
                    throw new InvalidOperationException("Unknown login mode: " + _options.Mode.ToString());
            }

            if (_options.Cache)
            {
                saveSessionCaches();
            }
        }

        private XboxAuthClient initializeAuthClient()
        {
            return new XboxAuthClient(HttpHelper.SharedHttpClient);
        }

        private XboxSignedClient initializeSignedClient()
        {
            return new XboxSignedClient(HttpHelper.SharedHttpClient);
        }

        private void loadSessionCaches()
        {
            userToken = _sessionCache.XboxUserToken;
            deviceToken = _sessionCache.XboxDeviceToken;
            titleToken = _sessionCache.XboxTitleToken;
            xstsToken = _sessionCache.XboxXstsToken;
        }

        private void saveSessionCaches()
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

        private async Task basicAuth(XboxAuthClient authClient)
        {
            Debug.Assert(_options.AccessToken != null);

            userToken = await authClient.RequestUserToken(_options.AccessToken);
            xstsToken = await authClient.RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken.Token,
                RelyingParty = _options.RelyingParty,
                DeviceToken = deviceToken?.Token,
                TitleToken = null,
                OptionalDisplayClaims = null
            });
        }

        private async Task fullAuth(XboxAuthClient authClient, XboxSignedClient signedClient)
        {
            Debug.Assert(_options.AccessToken != null);

            userToken = await signedClient.RequestSignedUserToken(new XboxSignedUserTokenRequest
            {
                AccessToken = _options.AccessToken,
                TokenPrefix = _options.TokenPrefix
            });
            deviceToken = await signedClient.RequestDeviceToken(new XboxDeviceTokenRequest
            {
                DeviceType = _options.DeviceType,
                DeviceVersion = _options.DeviceVersion
            });
            //titleToken = await _authClient.RequestTitleToken(new XboxTitleTokenRequest
            //{
            //    AccessToken = accessToken,
            //    DeviceToken = deviceToken.Token
            //});
            xstsToken = await authClient.RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken.Token,
                DeviceToken = deviceToken?.Token,
                TitleToken = titleToken?.Token,
                RelyingParty = _options.RelyingParty,
            });
        }

        private async Task<XboxSisuResponse> sisuAuth(XboxSignedClient signedClient)
        {
            Debug.Assert(_options.AccessToken != null);
            
            var result = await signedClient.SisuAuth(new XboxSisuAuthRequest
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
            ConsolePrinter.Print(userToken);
            Console.WriteLine("DeviceToken: ");
            ConsolePrinter.Print(deviceToken);
            Console.WriteLine("TitleToken: ");
            ConsolePrinter.Print(titleToken);
            Console.WriteLine("XstsToken: ");
            ConsolePrinter.Print(xstsToken);
        }
    }
}