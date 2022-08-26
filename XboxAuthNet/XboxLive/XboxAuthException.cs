﻿using System;
using System.Linq;
using System.Text.Json;
using XboxAuthNet.XboxLive.Entity;

namespace XboxAuthNet.XboxLive
{
    public class XboxAuthException : Exception
    {
        public XboxAuthException(string message, int statusCode) : base(message) =>
            StatusCode = statusCode;

        public XboxAuthException(string? error, string? message, string? redirect, int statusCode) : base(CreateMessageFromError(error, message, redirect)) =>
            (Error, ErrorMessage, Redirect, StatusCode) = (error, message, redirect, statusCode);

        private static string CreateMessageFromError(params string?[] inputs)
        {
            return string.Join(", ", inputs.Where(x => !string.IsNullOrEmpty(x)));
        }

        public int StatusCode { get; private set; }

        // https://github.com/microsoft/xbox-live-api/blob/730f579d41b64df5b57b52e629d12f23c6fb64ac/Source/Shared/errors_legacy.h#L924
        public string? Error { get; private set; }

        public string? ErrorMessage { get; private set; }

        public string? Redirect { get; private set; }

        public static XboxAuthException FromResponseBody(string responseBody, int statusCode)
        {
            try
            {
                var errRes = JsonSerializer.Deserialize<XboxErrorResponse>(responseBody);

                if (string.IsNullOrEmpty(errRes?.XErr) && string.IsNullOrEmpty(errRes?.Message))
                    throw new FormatException();

                return new XboxAuthException(errRes?.XErr, errRes?.Message, errRes?.Redirect, statusCode);
            }
            catch (JsonException)
            {
                throw new FormatException();
            }
        }
    }
}
