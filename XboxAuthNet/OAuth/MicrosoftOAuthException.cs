using System;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthException : Exception
    {
        public MicrosoftOAuthException(string? message)
            : base(message)
        {

        }

        public MicrosoftOAuthException(string? message, Exception inner)
            : base(message, inner)
        {
            
        }

        public MicrosoftOAuthException(string? error, string? errorDes, int[]? codes)
            : base(error)
        {
            this.Error = error;
            this.ErrorDescription = errorDes;
            this.ErrorCodes = codes;
        }

        public MicrosoftOAuthException(MicrosoftOAuthResponse oauthResponse)
            : base(oauthResponse.Error)
        {
            this.Response = oauthResponse;
            this.Error = Response.Error;
            this.ErrorDescription = oauthResponse.ErrorDescription;
            this.ErrorCodes = oauthResponse.ErrorCodes;
        }

        public MicrosoftOAuthResponse? Response { get; }
        public string? Error { get; }
        public string? ErrorDescription { get; }
        public int[]? ErrorCodes { get; }
    }
}