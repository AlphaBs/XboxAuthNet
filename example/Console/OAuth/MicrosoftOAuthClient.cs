using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNetConsole.OAuth
{
    public class MicrosoftOAuthClient
    {
        private readonly string _clientId;
        private readonly string _scopes;
        private readonly HttpClient _httpClient;

        public MicrosoftOAuthClient(
            string clientId,
            string scopes,
            HttpClient httpClient) =>
            (_clientId, _scopes, _httpClient) = (clientId, scopes, httpClient);

        private MicrosoftOAuthCodeApiClient? _apiClient;
        public MicrosoftOAuthCodeApiClient ApiClient => _apiClient ??= createApiClient();

        private MicrosoftOAuthCodeFlow? _codeFlow;
        public MicrosoftOAuthCodeFlow CodeFlow => _codeFlow ??= createCodeFlow();

        private MicrosoftOAuthCodeApiClient createApiClient()
        {
            return new MicrosoftOAuthCodeApiClient(_clientId, _scopes, _httpClient);
        }

        private MicrosoftOAuthCodeFlow createCodeFlow()
        {
            return new MicrosoftOAuthCodeFlowBuilder(ApiClient).Build();
        }
    }
}