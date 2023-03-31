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

        public MicrosoftOAuthCodeFlowBuilder WithUITitle(string title)
        {
            uiOptions ??= createDefaultWebUIOptions();
            uiOptions.Title = title;
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

        public MicrosoftOAuthCodeFlow Build()
        {
            uriChecker ??= createDefaultUriChecker();
            webUI ??= createDefaultWebUIForPlatform();
            return new MicrosoftOAuthCodeFlow(_apiClient, webUI, uriChecker);
        }

        private IMicrosoftOAuthUriChecker createDefaultUriChecker()
        {
            return new MicrosoftOAuthUriChecker();
        }

        private IWebUI createDefaultWebUIForPlatform()
        {
            this.uiOptions ??= createDefaultWebUIOptions();
            return PlatformManager.CurrentPlatform.CreateWebUI(uiOptions);
        }

        private WebUIOptions createDefaultWebUIOptions() => new WebUIOptions
        {
            ParentObject = null,
            SynchronizationContext = SynchronizationContext.Current
        };
    }
}