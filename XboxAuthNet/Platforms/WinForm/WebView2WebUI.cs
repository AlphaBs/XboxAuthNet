﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.Models;
using Microsoft.Web.WebView2.Core;

namespace XboxAuthNet.Platforms.WinForm
{
#if NET5_WIN
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    internal class WebView2WebUI : IWebUI
    {
        private readonly object? _parent;
        private readonly SynchronizationContext? _synchronizationContext;

        public WebView2WebUI(WebUIOptions options)
        {
            _parent = options.ParentObject;
            _synchronizationContext = options.SynchronizationContext;
        }

        public async Task<MicrosoftOAuthCode> DisplayDialogAndInterceptUri(
            Uri uri, 
            IMicrosoftOAuthUriChecker uriChecker, 
            CancellationToken cancellationToken)
        {
            MicrosoftOAuthCode? result = null;
            await UIThreadHelper.InvokeUIActionOnSafeThread(() =>
            {
                using (var form = new WinFormsPanelWithWebView2(_parent))
                {
                    result = form.DisplayDialogAndInterceptUri(uri, uriChecker, cancellationToken);
                }
            }, _synchronizationContext, cancellationToken);
            return result!;
        }

        public async Task DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken)
        {
            await UIThreadHelper.InvokeUIActionOnSafeThread(() =>
            {
                using (var form = new WinFormsPanelWithWebView2(_parent))
                {
                    form.DisplayDialogAndNavigateUri(uri, cancellationToken);
                }
            }, _synchronizationContext, cancellationToken);
        }

        public static bool IsWebView2Available()
        {
            try
            {
                string wv2Version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                return !string.IsNullOrEmpty(wv2Version);
            }
            catch (WebView2RuntimeNotFoundException)
            {
                return false;
            }
            catch (Exception ex) when (ex is BadImageFormatException || ex is DllNotFoundException)
            {
                return false;
                //throw new MsalClientException(MsalError.WebView2LoaderNotFound, MsalErrorMessage.WebView2LoaderNotFound, ex);
            }
        }
    }
}
