using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using System.IO;

namespace XboxAuthNet.Utils
{
    public class KeyPairGeneratorFactory
    {
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
