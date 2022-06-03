using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XboxAuthNet.XboxLive
{
    public class XboxAuthException : Exception
    {
        public XboxAuthException(string? message)
            : base(message)
        {
            
        }

        public XboxAuthException(string? message, Exception? inner)
            : base(message, inner)
        {
            
        }

        public XboxAuthException(string? errorCode, string? responseMessage)
            : base(responseMessage)
        {
            this.ErrorCode = errorCode;
            this.ResponseMessage = responseMessage;
        }

        public string? ErrorCode { get; }
        public string? ResponseMessage { get; }
    }
}
