using Org.BouncyCastle.Crypto;

namespace XboxAuthNet.Utils
{
    public class FixedAsymmetricKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
    {
        private readonly AsymmetricCipherKeyPair keypair;

        public FixedAsymmetricKeyPairGenerator(AsymmetricCipherKeyPair keypair)
        {
            this.keypair = keypair;
        }

        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            return keypair;
        }

        public void Init(KeyGenerationParameters parameters)
        {
            return;
        }
    }
}
