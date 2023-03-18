using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive;
using XboxAuthNet.XboxLive.Pop;

namespace XboxAuthNet.Test
{
    public class TestXboxSecureAuth
    {
        public async Task TestRequestSignerINFINITELY()
        {
            var httpClient = new HttpClient();
            var auth = new XboxSecureAuth(
                httpClient, 
                new XboxSisuRequestSigner(
                    new DefaultECSigner()));

            while (true)
            {
                try
                {
                    var result = await auth.RequestDeviceToken(XboxDeviceTypes.Win32, "0.0.0");
                    Console.WriteLine(result.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                await Task.Delay(1000 * 1);
            }
        }
    }
}
