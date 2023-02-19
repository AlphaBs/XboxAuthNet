using System;
using System.Threading;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthCodeFlowBuilder
    {
        private readonly MicrosoftOAuthCodeApiClient _apiClient;

        public MicrosoftOAuthCodeFlowBuilder(MicrosoftOAuthCodeApiClient apiClient)
        {
            this._apiClient = apiClient;
        }

        private WebUIOptions? uiOptions;
        private IWebUI? webUI;
        private IMicrosoftOAuthUriChecker? uriChecker;

        public MicrosoftOAuthCodeFlowBuilder WithUIParent(object parent)
        {
            uiOptions ??= createDefaultWebUIOptions();
            uiOptions.ParentObject = parent;
            return this;
        }

        public MicrosoftOAuthCodeFlowBuilder WithUIOptions(WebUIOptions options)
        {
            this.uiOptions = options;
            return this;
        }

        public MicrosoftOAuthCodeFlowBuilder WithWebUI(IWebUI ui)
        {
            this.webUI = ui;
            return this;
        }

        public MicrosoftOAuthCodeFlowBuilder WithWebUI(Func<WebUIOptions, IWebUI> factory)
        {
            this.uiOptions ??= createDefaultWebUIOptions();
            WithWebUI(factory.Invoke(this.uiOptions));
            return this;
        }

        public MicrosoftOAuthCodeFlowBuilder WithUriChecker(MicrosoftOAuthUriChecker checker)
        {
            this.uriChecker = checker;
            return this;
        }

        private WebUIOptions createDefaultWebUIOptions() => new WebUIOptions
        {
            ParentObject = null,
            SynchronizationContext = SynchronizationContext.Current
        };

        private IWebUI? createDefaultWebUIForPlatform()
        {
            IWebUI? ui = null;
            this.uiOptions ??= createDefaultWebUIOptions();

#if ENABLE_WEBVIEW2
            ui = new XboxAuthNet.Platforms.WinForm.WebView2WebUI(uiOptions);
#endif

            return ui;
        }

        private IMicrosoftOAuthUriChecker createDefaultUriChecker()
        {
            return new MicrosoftOAuthUriChecker();
        }

        public MicrosoftOAuthCodeFlow Build()
        {
            if (uriChecker == null)
            {
                uriChecker = createDefaultUriChecker();
            }

            if (webUI == null)
            {
                webUI = createDefaultWebUIForPlatform();

                if (webUI == null)
                {
                    throw new InvalidOperationException("Set WebUI instance");
                }
            }

            return new MicrosoftOAuthCodeFlow(_apiClient, webUI, uriChecker);
        }
    }
}