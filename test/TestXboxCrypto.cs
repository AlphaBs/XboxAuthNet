using System;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive;

namespace XboxAuthNet.Test
{
    public class TestXboxCrypto
    {
        public async Task TestSignedRequestINFINITELY()
        {
            var httpClient = new HttpClient();
            var client = new XboxSignedClient(httpClient);

            while (true)
            {
                try
                {
                    var result = await client.RequestDeviceToken(XboxDeviceTypes.Win32, "0.0.0");
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
