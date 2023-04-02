using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.Models;
using XboxAuthNetConsole.Options;
using XboxAuthNetConsole.Cache;
using XboxAuthNetConsole.Printer;

namespace XboxAuthNetConsole.OAuth
{
    public class MicrosoftOAuthCommand : ICommand
    {
        private readonly SessionCache _sessionCache;
        private readonly MicrosoftOAuthOptions _options;

        public MicrosoftOAuthCommand(
            MicrosoftOAuthOptions options,
            SessionCache sessionCache)
        {
            _options = options;
            _sessionCache = sessionCache;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            Console.WriteLine("Microsoft OAuth scopes: " + _options.Scopes);
            MicrosoftOAuthResponse response;

            switch (_options.Mode)
            {
                case MicrosoftOAuthMode.Auto:
                    response = await autoAuth(cancellationToken);
                    break;
                case MicrosoftOAuthMode.Interactive:
                    response = await interactiveAuth(cancellationToken);
                    break;
                case MicrosoftOAuthMode.Silent:
                    response = await silentAuth(cancellationToken);
                    break;
                case MicrosoftOAuthMode.Signout:
                    response = await signout(cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException("Unknown login mode: " + _options.Mode.ToString());
            }

            if (_options.Cache)
                _sessionCache.MicrosoftOAuth = response;
            ConsolePrinter.Print(response);
        }

        private async Task<MicrosoftOAuthResponse> autoAuth(CancellationToken cancellationToken)
        {
            Console.WriteLine("Auto login mode");
            try
            {
                return await silentAuth(cancellationToken);
            }
            catch (MicrosoftOAuthException)
            {
                return await interactiveAuth(cancellationToken);
            }
        }

        private async Task<MicrosoftOAuthResponse> interactiveAuth(CancellationToken cancellationToken)
        {
            Console.WriteLine("Start Microsoft OAuth interactive login");
            var codeFlow = initializeCodeFlow();
            return await codeFlow.Authenticate(new MicrosoftOAuthParameters
            {
                LoginHint = _options.LoginHint,
                Prompt = _options.Prompt
            }, cancellationToken);
        }

        private async Task<MicrosoftOAuthResponse> silentAuth(CancellationToken cancellationToken)
        {
            Console.WriteLine("Start Microsoft OAuth silent login");

            var refreshToken = _options.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
            {  
                refreshToken = _sessionCache.MicrosoftOAuth?.RefreshToken;
            }
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new MicrosoftOAuthException(
                    "No refresh token was specified. \n" + 
                    "Specify '--refreshToken' or login as interactive mode first.", -1);
            }

            var apiClient = initializeApiClient();
            return await apiClient.RefreshToken(refreshToken, cancellationToken);
        }

        private async Task<MicrosoftOAuthResponse> signout(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private MicrosoftOAuthCodeFlow initializeCodeFlow()
        {
            var apiClient = initializeApiClient();
            return new MicrosoftOAuthCodeFlowBuilder(apiClient)
                .Build();
        }

        private MicrosoftOAuthCodeApiClient initializeApiClient()
        {
            if (string.IsNullOrEmpty(_options.ClientId))
                throw new InvalidOperationException("ClientId was null. Specify '--clientId' option or set value of settings.json file");
            if (string.IsNullOrEmpty(_options.Scopes))
                throw new InvalidOperationException("Scopes was null. Specify '--scopes' option or set value of settings.json file");
                
            return new MicrosoftOAuthCodeApiClient(_options.ClientId, _options.Scopes, HttpHelper.SharedHttpClient);
        }
    }
}