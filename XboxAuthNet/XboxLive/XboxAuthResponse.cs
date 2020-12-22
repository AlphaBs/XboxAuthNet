using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxAuthNet.XboxLive
{
    public class XboxAuthResponse
    {
        public string UserXUID { get; set; }
        public string UserHash { get; set; }
        public string XSTSToken { get; set; }
        public string ExpireOn { get; set; }
    }
}
