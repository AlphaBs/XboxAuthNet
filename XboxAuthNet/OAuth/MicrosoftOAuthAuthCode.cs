using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthAuthCode
    {
        public string Code { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }

        // error description
        // https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow#error-codes-for-authorization-endpoint-errors

        public bool IsSuccess 
           => !string.IsNullOrEmpty(Code)
            && string.IsNullOrEmpty(Error);

        public bool IsEmpty 
            => string.IsNullOrEmpty(Code) 
            && string.IsNullOrEmpty(Error)
            && string.IsNullOrEmpty(ErrorDescription);
    }
}
