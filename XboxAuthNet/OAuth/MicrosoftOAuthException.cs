using System;

namespace XboxAuthNet.OAuth
{
    public class MicrosoftOAuthException : Exception
    {
        public MicrosoftOAuthException()
        {

        }

        public MicrosoftOAuthException(string message)
            : base(message)
        {
            
        }

        public MicrosoftOAuthException(MicrosoftOAuthResponse oauth)
        {
            this.Error = oauth.Error;
            this.ErrorDescription = oauth.ErrorDescription;
            this.ErrorCodes = oauth.ErrorCodes;
        }

        public string? Error { get; private set; }
        public string? ErrorDescription { get; private set; }
        public int[]? ErrorCodes { get; private set; }
    }
}