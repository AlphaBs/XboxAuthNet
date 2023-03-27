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

        private string TokenPrefix => _options.TokenPrefix ?? XboxAuthConstants.XboxTokenPrefix;
        private string AccessToken => _options.AccessToken ?? throw new InvalidOperationException();
        private string RelyingParty => _options.RelyingParty ?? XboxAuthConstants.XboxLiveRelyingParty;
        private string ClientId => _options.ClientId ?? throw new InvalidOperationException();
        private string DeviceType => _options.DeviceType ?? XboxDeviceTypes.Nintendo;
        private string DeviceVersion => _options.DeviceVersion ?? "0.0.0";

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
                    printSisuResponse(sisuResponse);
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
            userToken = await _authClient.RequestUserToken(AccessToken);
            xstsToken = await _authClient.RequestXsts(new XboxXstsRequest
            {
                UserToken = userToken.Token,
                RelyingParty = RelyingParty,
                DeviceToken = deviceToken?.Token,
                TitleToken = null,
                OptionalDisplayClaims = null
            });
        }

        private async Task fullAuth()
        {
            userToken = await _authClient.RequestSignedUserToken(new XboxSignedUserTokenRequest
            {
                AccessToken = AccessToken,
                TokenPrefix = TokenPrefix
            });
            deviceToken = await _authClient.RequestDeviceToken(new XboxDeviceTokenRequest
            {
                DeviceType = DeviceType,
                DeviceVersion = DeviceVersion
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
                RelyingParty = RelyingParty,
            });
        }

        private async Task<XboxSisuResponse> sisuAuth()
        {
            var result = await _authClient.SisuAuth(new XboxSisuAuthRequest
            {
                TokenPrefix = TokenPrefix,
                AccessToken = AccessToken,
                RelyingParty = RelyingParty,
                ClientId = ClientId,
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

        private void printResponse(XboxAuthResponse? response)
        {
            if (response == null)
            {
                Console.WriteLine("null");
                return;
            }

            Console.WriteLine($"Token: {response.Token}");
            Console.WriteLine($"ExpireOn: {response.ExpireOn}");
            Console.WriteLine($"XuiClaims.AgeGroup: {response.XuiClaims?.AgeGroup}");
            Console.WriteLine($"XuiClaims.Gamertag: {response.XuiClaims?.Gamertag}");
            Console.WriteLine($"XuiClaims.Privileges: {response.XuiClaims?.Privileges}");
            Console.WriteLine($"XuiClaims.UserHash: {response.XuiClaims?.UserHash}");
            Console.WriteLine($"XuiClaims.UserSettingsRestrictions: {response.XuiClaims?.UserSettingsRestrictions}");
            Console.WriteLine($"XuiClaims.UserTitleRestrictions: {response.XuiClaims?.UserTitleRestrictions}");
            Console.WriteLine($"XuiClaims.XboxUserId: {response.XuiClaims?.XboxUserId}");
        }

        private void printSisuResponse(XboxSisuResponse response)
        {
            Console.WriteLine("Xbox Sisu Auth Success");
            Console.WriteLine($"Sandbox: {response.Sandbox}");
            Console.WriteLine($"UseModernGamertag: {response.UseModernGamertag}");
            Console.WriteLine($"WebPage: {response.WebPage}");
            Console.WriteLine("UserToken:");
            printResponse(response.UserToken);
            Console.WriteLine("DeviceToken:");
            Console.WriteLine(response.DeviceToken);
            Console.WriteLine("TitleToken: ");
            printResponse(response.TitleToken);
            Console.WriteLine("XstsToken: ");
            printResponse(response.AuthorizationToken);
        }
    }
}