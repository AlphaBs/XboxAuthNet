using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxAuthNet
{
    internal class IssueLink
    {
        public const string
            UserTokenIssue = "https://bit.ly/xr-xbl-auth-user-token-issue",
            CreateIssue = "https://bit.ly/xr-xbl-auth-create-issue",
            UnauthorizedActivityError = "https://bit.ly/xr-xbl-auth-err-activity",
            TwoFactorAuthenticationError = "https://bit.ly/xr-xbl-auth-err-2fa",

            _dummy = "";
    }
}
