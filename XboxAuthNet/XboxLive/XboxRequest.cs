using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace XboxAuthNet.XboxLive.Entity
{
    public class XboxRequest
    {
        public static async Task<T> Send<T>(HttpClient httpClient, HttpRequestMessage req)
        {
            req.Headers.Add("Accept", "application/json");
            req.Headers.TryAddWithoutValidation("User-Agent", HttpUtil.UserAgent);
            req.Headers.Add("Accept-Language", "en-US");
            req.Headers.Add("Cache-Control", "no-store, must-revalidate, no-cache");

            var res = await httpClient.SendAsync(req);
            return await HandleResponse<T>(res);
        }

        public static async Task<T> HandleResponse<T>(HttpResponseMessage res)
        {
            var resBody = await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            try
            {
                res.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<T>(resBody)
                    ?? throw new JsonException();
            }
            catch (Exception ex) when (
                ex is JsonException ||
                ex is HttpRequestException)
            {
                try
                {
                    throw XboxAuthException.FromResponseBody(resBody, (int)res.StatusCode);
                }
                catch (FormatException)
                {
                    try
                    {
                        throw XboxAuthException.FromResponseHeaders(res.Headers, (int)res.StatusCode);
                    }
                    catch (FormatException)
                    {
                        throw new XboxAuthException($"{(int)res.StatusCode}: {res.ReasonPhrase}", (int)res.StatusCode);
                    }
                }
            }
        }
    }
}
