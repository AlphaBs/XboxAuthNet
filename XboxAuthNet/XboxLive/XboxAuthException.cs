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
            this.ErrorCode = tryConvertToHexErrorCode(errorCode);
            this.ResponseMessage = responseMessage;
        }

        private string? tryConvertToHexErrorCode(string? errorCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(errorCode))
                { 
                    var errorInt = long.Parse(errorCode);
                    errorCode = errorInt.ToString("x");
                }
                return errorCode;
            }
            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
            {
                return errorCode;
            }
        }

        public string? ErrorCode { get; }
        public string? ResponseMessage { get; }
    }
}
