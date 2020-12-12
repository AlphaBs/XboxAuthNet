using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxAuthNet.OAuth
{
    public class LogUserResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string ExpireIn { get; set; }
        public string Scope { get; set; }
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
    }
}
