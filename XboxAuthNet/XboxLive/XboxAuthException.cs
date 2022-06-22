using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace XboxAuthNet.XboxLive
{
    public class XboxAuthException : Exception
    {
        public XboxAuthException(string message, int statusCode) : base(message) =>
            StatusCode = statusCode;

        public XboxAuthException(string? error, string? message, string? redirect, int statusCode) : base(CreateMessageFromError(error, message)) =>
            (Error, ErrorMessage, Redirect, StatusCode) = (error, message, redirect, statusCode);

        public XboxAuthException(string? message)
            : base(message)
        {
            
        }

        private static string CreateMessageFromError(string? error, string? errorDes)
        {
            return string.Join(", ", new string?[] { error, errorDes }.Where(x => !string.IsNullOrEmpty(x)));
        }

        public int StatusCode { get; private set; }

        public string? Error { get; private set; }

        public string? ErrorMessage { get; private set; }

        public string? Redirect { get; private set; }

        public static XboxAuthException FromResponseBody(string responseBody, int statusCode)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                string? error = null;
                string? errorMessage = null;
                string? redirect = null;

                if (root.TryGetProperty("XErr", out var errProp) &&
                    errProp.ValueKind == JsonValueKind.String)
                    error = errProp.GetString();
                if (root.TryGetProperty("Message", out var messageProp) &&
                    messageProp.ValueKind == JsonValueKind.String)
                    errorMessage = messageProp.GetString();
                if (root.TryGetProperty("Redirect", out var redirectProp) &&
                    redirectProp.ValueKind == JsonValueKind.String)
                    redirect = redirectProp.GetString();

                if (string.IsNullOrEmpty(error) && string.IsNullOrEmpty(errorMessage))
                    throw new FormatException();

                return new XboxAuthException(tryConvertToHexErrorCode(error), errorMessage, redirect, statusCode);
            }
            catch (JsonException)
            {
                throw new FormatException();
            }
        }

        private static string? tryConvertToHexErrorCode(string? errorCode)
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
            catch (FormatException)
            {
                return errorCode;
            }
            catch (OverflowException)
            {
                return errorCode;
            }
        }
    }
}
