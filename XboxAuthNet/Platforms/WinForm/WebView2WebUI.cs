using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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

        public async Task<MicrosoftOAuthCode> DisplayDialogAndInterceptUri(Uri uri, IMicrosoftOAuthUriChecker uriChecker, CancellationToken cancellationToken)
        {
            MicrosoftOAuthCode? result = null;
            await invokeOnSafeThread(() =>
            {
                using (var form = new WinFormsPanelWithWebView2(_parent))
                {
                    result = form.DisplayDialogAndInterceptUri(uri, uriChecker, cancellationToken);
                }
            }, cancellationToken);
            return result!;
        }

        public async Task DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken)
        {
            await invokeOnSafeThread(() =>
            {
                using (var form = new WinFormsPanelWithWebView2(_parent))
                {
                    form.DisplayDialogAndNavigateUri(uri, cancellationToken);
                }
            }, cancellationToken);
        }

        private async Task invokeOnSafeThread(Action action, CancellationToken cancellationToken)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
            {
                await invokeWithinMtaThread(action, cancellationToken);
            }
            else
            {
                action.Invoke();
            }
        }

        private async Task invokeWithinMtaThread(Action action, CancellationToken cancellationToken)
        {
            if (_synchronizationContext != null)
            {
                var actionWithTcs = new Action<object?>((tcs) =>
                {
                    try
                    {
                        action.Invoke();
                        ((TaskCompletionSource<object?>)tcs!).TrySetResult(null);
                    }
                    catch (Exception e)
                    {
                        // Need to catch the exception here and put on the TCS which is the task we are waiting on so that
                        // the exception comming out of Authenticate is correctly thrown.
                        ((TaskCompletionSource<object>)tcs!).TrySetException(e);
                    }
                });

                var tcs2 = new TaskCompletionSource<object?>();

                _synchronizationContext.Post(
                    new SendOrPostCallback(actionWithTcs), tcs2);
                await tcs2.Task.ConfigureAwait(false);
            }
            else
            {
                using (var staTaskScheduler = new StaTaskScheduler(1))
                {
                    try
                    {
                        Task.Factory.StartNew(
                            action,
                            cancellationToken,
                            TaskCreationOptions.None,
                            staTaskScheduler).Wait();
                    }
                    catch (AggregateException ae)
                    {
                        // Any exception thrown as a result of running task will cause AggregateException to be thrown with
                        // actual exception as inner.
                        Exception innerException = ae.InnerExceptions[0];

                        // In MTA case, AggregateException is two layer deep, so checking the InnerException for that.
                        if (innerException is AggregateException)
                        {
                            innerException = ((AggregateException)innerException).InnerExceptions[0];
                        }

                        throw innerException;
                    }
                }
            }
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
