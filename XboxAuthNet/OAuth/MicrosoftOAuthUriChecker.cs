using System;
using System.Web;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthUriChecker : IMicrosoftOAuthUriChecker
    {
        public MicrosoftOAuthCodeCheckResult CheckOAuthUri(Uri uri)
        {
            var query = HttpUtility.ParseQueryString(uri.Query);
            var authCode = new MicrosoftOAuthCode
            {
                Code = query["code"],
                IdToken = query["id_token"],
                State = query["state"],
                Error = query["error"],
                ErrorDescription = HttpUtility.UrlDecode(query["error_description"])
            };

            return new MicrosoftOAuthCodeCheckResult(!authCode.IsEmpty, authCode);
        }
    }
}
