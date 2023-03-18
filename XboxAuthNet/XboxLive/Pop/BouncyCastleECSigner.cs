using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using XboxAuthNet.Utils;

namespace XboxAuthNet.XboxLive.Pop
{
    public class BouncyCastleECSigner : IPopCryptoProvider
    {
        private readonly ECPublicKeyParameters _publicKey;
        private readonly ECPrivateKeyParameters _privateKey;

        public BouncyCastleECSigner(
            ECPublicKeyParameters publicKey, 
            ECPrivateKeyParameters privateKey)
        {
            this._publicKey = publicKey;
            this._privateKey = privateKey;
        }

        private object? _proofKey;
        public object ProofKey => _proofKey ??= generateNewProofKey();

        private object generateNewProofKey()
        {
            return new
            {
                kty = "EC",
                x = Base64UrlHelpers.Encode(_publicKey.Q.AffineXCoord.ToBigInteger().ToByteArray()),
                y = Base64UrlHelpers.Encode(_publicKey.Q.AffineYCoord.ToBigInteger().ToByteArray()),
                crv = "P-256",
                alg = "ES256",
                use = "sig"
            };
        }

        public byte[] Sign(byte[] bytes)
        {
            var signer = SignerUtilities.GetSigner("SHA256WITHPLAIN-ECDSA");
            signer.Init(true, _privateKey);
            signer.BlockUpdate(bytes, 0, bytes.Length);
            return signer.GenerateSignature();
        }
    }
}
