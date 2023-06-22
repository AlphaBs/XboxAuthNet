using System.Threading.Tasks;

namespace XboxAuthNet.Test
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var test = new TestXboxCrypto();
            await test.TestSignedRequestINFINITELY();
        }
    }
}