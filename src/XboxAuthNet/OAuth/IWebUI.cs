// This code is from MSAL.NET
using System;
using System.Threading;
using System.Threading.Tasks;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNet.OAuth
{
    public interface IWebUI
    {
        Task<MicrosoftOAuthCode> DisplayDialogAndInterceptUri(Uri uri, IMicrosoftOAuthUriChecker uriChecker, CancellationToken cancellationToken);
        Task DisplayDialogAndNavigateUri(Uri uri, CancellationToken cancellationToken);
    }
}