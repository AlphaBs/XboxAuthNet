using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace XboxAuthNet
{
    public class HttpUtil
    {
        public const string UserAgent = "Mozilla/5.0 (XboxReplay; XboxLiveAuth/3.0)" +
                                        "AppleWebKit/537.36 (KHTML, like Gecko)" +
                                        "Chrome/71.0.3578.98 Safari/537.36";

        public static string GetQueryString(Dictionary<string, string> queries)
        {
            return string.Join("&",
                queries.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
        }

        public static HttpWebRequest CreateDefaultRequest(string url, Dictionary<string, string> queries)
        {
            return CreateDefaultRequest(url + "?" + GetQueryString(queries));
        }

        public static HttpWebRequest CreateDefaultRequest(string url)
        {
            var req = WebRequest.CreateHttp(url);
            req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
            req.Headers[HttpRequestHeader.AcceptLanguage] = "en-US";
            req.Headers[HttpRequestHeader.UserAgent] = UserAgent;

            req.AutomaticDecompression = DecompressionMethods.GZip;
            req.Method = "GET";
            return req;
        }

        public static string ReadResponse(WebResponse res)
        {
            using (var resStream = res.GetResponseStream())
            using (var sr = new StreamReader(resStream))
            {
                return sr.ReadToEnd();
            }
        }

        public static void WriteRequest(HttpWebRequest req, string body)
        {
            using (var reqStream = req.GetRequestStream())
            using (var sw = new StreamWriter(reqStream))
            {
                sw.Write(body);
            }
        }

        public static NameValueCollection ParseQuery(string q)
        {
            return HttpUtility.ParseQueryString(q);
        }

        public static string UrlEncode(string q)
        {
            return HttpUtility.UrlEncode(q);
        }
    }

    public static class HttpWebResponseExt
    {
        public static HttpWebResponse GetResponseNoException(this HttpWebRequest req)
        {
            try
            {
                return (HttpWebResponse)req.GetResponse();
            }
            catch (WebException we)
            {
                var resp = we.Response as HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
    }
}
