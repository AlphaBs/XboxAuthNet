using System.Security.Cryptography;

namespace XboxAuthNet.XboxLive.Crypto;

// https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/main/tests/Microsoft.Identity.Test.Common/Core/Helpers/ECDCertificatePopCryptoProvider.cs#L47
public class ECDCertificatePopCryptoProvider : IPopCryptoProvider
{
    private ProofKeyBody? _proofKey;
    public object ProofKey => _proofKey ??= generateNewProofKey();

    private ECDsa _signer;

    public ECDCertificatePopCryptoProvider()
    {
        var ecCurve = ECCurve.NamedCurves.nistP256;
        _signer = ECDsa.Create(ecCurve);
    }

    private ProofKeyBody generateNewProofKey()
    {
        var parameters = _signer.ExportParameters(false);
        return new ProofKeyBody
        {
            KeyType = "EC",
            X = parameters.Q.X != null ? Base64UrlHelper.Encode(parameters.Q.X) : null,
            Y = parameters.Q.Y != null ? Base64UrlHelper.Encode(parameters.Q.Y) : null,
            Curve = "P-256",
            Algorithm = "ES256",
            Use = "sig"
        };
    }

    public byte[] Sign(byte[] data)
    {
        return _signer.SignData(data, HashAlgorithmName.SHA256);
    }
}
