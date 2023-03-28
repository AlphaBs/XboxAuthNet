using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.Models;
using XboxAuthNetConsole.Options;
using XboxAuthNetConsole.Cache;

namespace XboxAuthNetConsole.OAuth
{
    public class MicrosoftOAuthCommand : ICommand
    {
        private readonly SessionCache _sessionCache;
        private readonly MicrosoftOAuthClient _client;
        private readonly MicrosoftOAuthOptions _options;

        public MicrosoftOAuthCommand(
            MicrosoftOAuthClient client,
            MicrosoftOAuthOptions options,
            SessionCache sessionCache)
        {
            _client = client;
            _options = options;
            _sessionCache = sessionCache;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
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
                default:
                    throw new InvalidOperationException("Invalid AuthMode: " + _options.Mode.ToString());
            }

            _sessionCache.MicrosoftOAuth = response;
            printResponse(response);
        }

        private async Task<MicrosoftOAuthResponse> autoAuth(CancellationToken cancellationToken)
        {
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
            return await _client.CodeFlow.Authenticate(new MicrosoftOAuthParameters
            {
                LoginHint = _options.LoginHint,
                Prompt = _options.Prompt
            }, cancellationToken);
        }

        private async Task<MicrosoftOAuthResponse> silentAuth(CancellationToken cancellationToken)
        {
            var refreshToken = _options.RefreshToken;
            if (string.IsNullOrEmpty(refreshToken))
            {  
                refreshToken = _sessionCache.MicrosoftOAuth?.RefreshToken;
            }
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new InvalidOperationException("Specify refresh token or login as interactive mode first. There is no cached refresh token.");
            }

            return await _client.ApiClient.RefreshToken(refreshToken, cancellationToken);
        }
    }
}