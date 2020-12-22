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
        public XboxAuthException(string message, string link)
            : base(message)
        {
            base.HelpLink = link;
        }

        public XboxAuthException(string message, string link, Exception inner)
            : base(message, inner)
        {
            base.HelpLink = link;
        }

        public XboxAuthException(string message, string link, HttpWebResponse res)
            : base($"{message} {res.StatusCode}: {res.StatusDescription}")
        {
            base.HelpLink = link;
            this.Response = res;
        }

        public HttpWebResponse Response { get; private set; }
    }
}
