using System.Text.Json;

namespace XboxAuthNetConsole
{
    public class JsonHelper
    {
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }
}