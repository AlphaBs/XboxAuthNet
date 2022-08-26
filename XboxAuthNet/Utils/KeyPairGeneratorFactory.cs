using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using System.IO;

namespace XboxAuthNet.Utils
{
    public class KeyPairGeneratorFactory
    {
        public static IAsymmetricCipherKeyPairGenerator CreateDefaultAsymmetricKeyPair()
        {
            var pem =
                "-----BEGIN EC PRIVATE KEY-----" +
                "MHcCAQEEIBmQFwiEgUsjIsFT2DUursdHKcr/Vmx6C2vuS7fBIMzLoAoGCCqGSM49" +
                "AwEHoUQDQgAEUeVH3ZwR4BYEUCCsRohY31SvwrJDztJzbSScHtZGybV/k+OkGvUv" +
                "SS3ZRNHdNJiJc4PFLJLFRj254lyHax66BA==" +
                "-----END EC PRIVATE KEY-----";
            return CreateAsymmetricKeyPairFromPemString(pem);
        }

        public static IAsymmetricCipherKeyPairGenerator CreateAsymmetricKeyPairFromPemFile(string path)
        {
            using var file = File.OpenText(path);
            return internalCreateAsymmetricKeyPair(file);
        }

        public static IAsymmetricCipherKeyPairGenerator CreateAsymmetricKeyPairFromPemString(string pem)
        {
            return internalCreateAsymmetricKeyPair(new StringReader(pem));
        }

        private static IAsymmetricCipherKeyPairGenerator internalCreateAsymmetricKeyPair(TextReader text)
        {
            var reader = new PemReader(text);
            var keyPair = (AsymmetricCipherKeyPair)reader.ReadObject();
            return new FixedAsymmetricKeyPairGenerator(keyPair);
        }
    }
}
