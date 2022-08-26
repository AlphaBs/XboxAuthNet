using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XboxAuthNet.XboxLive.Entity
{
    public class XErrJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value;
            if (reader.TokenType == JsonTokenType.String)
                value = reader.GetString();
            else if (reader.TokenType == JsonTokenType.Number)
                value = reader.GetInt64().ToString();
            else
                throw new JsonException();
            return tryConvertToHexErrorCode(value);
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

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
