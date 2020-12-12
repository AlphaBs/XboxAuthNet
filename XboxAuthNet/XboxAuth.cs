using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxAuthNet.Exchange;
using XboxAuthNet.OAuth;

namespace XboxAuthNet
{
    public class XboxAuth
    {
        public XboxAuthResponse Authenticate(string email, string pw, string XSTSRelyingParty=null)
        {
            var oauth = new XboxLiveOAuth();
            var preAuthRes = oauth.PreAuth();
            var logUserRes = oauth.LogUser(email, pw, preAuthRes);

            var exchanger = new XboxExchanger();
            var rpsRes = exchanger.ExchangeRpsTicketForUserToken(logUserRes.AccessToken);
            var xstsRes = exchanger.ExchangeTokensForXSTSIdentity(rpsRes.Token, null, null, XSTSRelyingParty, null);

            return xstsRes;
        }
    }
}
