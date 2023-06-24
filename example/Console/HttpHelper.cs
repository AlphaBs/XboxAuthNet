
namespace XboxAuthNetConsole
{
    public class HttpHelper
    {
        private static HttpClient? _httpClient;
        public static HttpClient SharedHttpClient => 
            _httpClient ??= new HttpClient();
    }
}