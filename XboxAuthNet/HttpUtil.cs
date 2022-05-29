using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace XboxAuthNet
{
    public class HttpUtil
    {
        public const string UserAgent = "Mozilla/5.0 (XboxReplay; XboxLiveAuth/3.0) " +
                                        "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                        "Chrome/71.0.3578.98 Safari/537.36";

        public static string GetQueryString(Dictionary<string, string?> queries)
        {
            return string.Join("&",
                queries.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
        }
    }
}
