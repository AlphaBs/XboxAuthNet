using System;
using System.Collections.Generic;
using System.Text;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNet.OAuth
{
    public interface IMicrosoftOAuthUriChecker
    {
        MicrosoftOAuthCodeCheckResult CheckOAuthUri(Uri uri);
    }
}
