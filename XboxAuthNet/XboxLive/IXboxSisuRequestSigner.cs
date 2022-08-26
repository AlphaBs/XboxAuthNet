using Org.BouncyCastle.Crypto;

namespace XboxAuthNet.XboxLive
{
    public interface IXboxSisuRequestSigner
    {
        object ProofKey { get; }
        string GenerateSignature(string reqUri, string token, string body);
    }
}
