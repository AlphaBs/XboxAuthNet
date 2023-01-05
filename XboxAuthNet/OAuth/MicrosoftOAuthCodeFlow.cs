﻿using System;
using System.Threading.Tasks;
using System.Threading;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthCodeFlow
    {
        public static MicrosoftOAuthCodeFlow CreateDefault(MicrosoftOAuthCodeApiClient client, object? parent = null)
        {
            IWebUI? ui = null;
            var uiOptions = new WebUIOptions
            {
                ParentObject = parent,
                SynchronizationContext = SynchronizationContext.Current
            };

#if ENABLE_WEBVIEW2
            ui = new XboxAuthNet.Platforms.WinForm.WebView2WebUI(uiOptions);
#endif

            if (ui == null)
            {
                throw new PlatformNotSupportedException();
            }

            var urlChecker = new MicrosoftOAuthUriChecker();
            return new MicrosoftOAuthCodeFlow(client, ui, urlChecker);
        }

        private readonly MicrosoftOAuthCodeApiClient _client;
        private readonly IWebUI _ui;
        private readonly IMicrosoftOAuthUriChecker _uriChecker;

        public MicrosoftOAuthCodeFlow(
            MicrosoftOAuthCodeApiClient client,
            IWebUI ui,
            IMicrosoftOAuthUriChecker urlChecker)
        {
            _client = client;
            _ui = ui;
            _uriChecker = urlChecker;
        }

        public Task<MicrosoftOAuthResponse> Authenticate(CancellationToken cancellationToken = default)
            => Authenticate(_client.CreateUrlForOAuth(), cancellationToken);

        public Task<MicrosoftOAuthResponse> Authenticate(MicrosoftOAuthParameters parameters,
            CancellationToken cancellationToken = default)
            => Authenticate(_client.CreateUrlForOAuth(parameters), cancellationToken);

        private async Task<MicrosoftOAuthResponse> Authenticate(string oauthUri,
            CancellationToken cancellationToken = default)
        {
            var authCode = await _ui.DisplayDialogAndInterceptUri(new Uri(oauthUri), _uriChecker, cancellationToken);

            if (!authCode.IsSuccess)
            {
                throw new AuthCodeException(authCode.Error, authCode.ErrorDescription);
            }

            return await _client.GetTokens(authCode, cancellationToken);
        }

        public Task<MicrosoftOAuthResponse> AuthenticateSilently(string refreshToken,
            CancellationToken cancellationToken = default)
        {
            return _client.RefreshToken(refreshToken, cancellationToken);
        }

        public async Task Signout(CancellationToken cancellationToken = default)
        {
            var url = _client.CreateUrlForSignout();
            await _ui.DisplayDialogAndNavigateUri(new Uri(url), cancellationToken);
        }
    }
}
