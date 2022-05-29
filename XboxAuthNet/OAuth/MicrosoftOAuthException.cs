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
    }
}