using System.Security.Cryptography;
using XboxAuthNet.Utils;

namespace XboxAuthNet.XboxLive.Pop
{
    // https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/9895855ac4fcf52893fbc2b06ee20ea3eda1549a/tests/Microsoft.Identity.Test.Common/Core/Helpers/ECDCertificatePopCryptoProvider.cs#L11
    public class DefaultECSigner : IPopCryptoProvider
    {
        private object? _proofKey;
        public object ProofKey => _proofKey ??= generateNewProofKey();

        private ECDsa _signer;

        public DefaultECSigner()
        {
            var ecCurve = ECCurve.NamedCurves.nistP256;
            _signer = ECDsa.Create(ecCurve);
        }

        private object generateNewProofKey()
        {
            var parameters = _signer.ExportParameters(false);
            return new
            {
                kty = "EC",
                x = parameters.Q.X != null ? Base64UrlHelpers.Encode(parameters.Q.X) : null,
                y = parameters.Q.Y != null ? Base64UrlHelpers.Encode(parameters.Q.Y) : null,
                crv = "P-256",
                alg = "ES256",
                use = "sig"
            };
        }

        public byte[] Sign(byte[] data)
        {
            return _signer.SignData(data, HashAlgorithmName.SHA256);
        }
    }
}