using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Text;

namespace XboxAuthNet.XboxLive
{
    public class ECXboxSisuRequestSigner : IXboxSisuRequestSigner
    {
        private readonly ECPublicKeyParameters _publicKey;
        private readonly ECPrivateKeyParameters _privateKey;

        public ECXboxSisuRequestSigner(
            ECPublicKeyParameters publicKey, 
            ECPrivateKeyParameters privateKey)
        {
            this._publicKey = publicKey;
            this._privateKey = privateKey;
        }

        private object? _proofKey;

        public object ProofKey
        {
            get
            {
                if (_proofKey == null)
                    _proofKey = createNewProofKey();
                return _proofKey;
            }
        }

        private object createNewProofKey()
        {
            return new
            {
                kty = "EC",
                x = base64url(_publicKey.Q.AffineXCoord.ToBigInteger().ToByteArray()),
                y = base64url(_publicKey.Q.AffineYCoord.ToBigInteger().ToByteArray()),
                crv = "P-256",
                alg = "ES256",
                use = "sig"
            };
        }

        public string GenerateSignature(string reqUri, string token, string body)
        {
            var timestamp = getWindowsTimestamp();
            var data = generatePayload(timestamp, reqUri, token, body);
            var signature = sign(timestamp, data);
            return Convert.ToBase64String(signature);
        }

        private byte[] generatePayload(ulong windowsTimestamp, string uri, string token, string payload)
        {
            var pathAndQuery = new Uri(uri).PathAndQuery;

            var allocSize =
                4 + 1 +
                8 + 1 +
                4 + 1 +
                pathAndQuery.Length + 1 +
                token.Length + 1 +
                payload.Length + 1;
            var bytes = new byte[allocSize];

            var policyVersion = BitConverter.GetBytes((int)1);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(policyVersion);
            Array.Copy(policyVersion, 0, bytes, 0, 4);

            var windowsTimestampBytes = BitConverter.GetBytes(windowsTimestamp);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(windowsTimestampBytes);
            Array.Copy(windowsTimestampBytes, 0, bytes, 5, 8);

            var strs =
                $"POST\0" +
                $"{pathAndQuery}\0" +
                $"{token}\0" +
                $"{payload}\0";
            var strsBytes = Encoding.ASCII.GetBytes(strs);
            Array.Copy(strsBytes, 0, bytes, 14, strsBytes.Length);

            return bytes;
        }

        private byte[] sign(ulong windowsTimestamp, byte[] bytes)
        {
            var signer = SignerUtilities.GetSigner("SHA256WITHPLAIN-ECDSA");
            signer.Init(true, _privateKey);
            signer.BlockUpdate(bytes, 0, bytes.Length);
            var signature = signer.GenerateSignature();

            var policyVersion = BitConverter.GetBytes((int)1);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(policyVersion);

            var windowsTimestampBytes = BitConverter.GetBytes(windowsTimestamp);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(windowsTimestampBytes);

            var header = new byte[signature.Length + 12];
            Array.Copy(policyVersion, 0, header, 0, 4);
            Array.Copy(windowsTimestampBytes, 0, header, 4, 8);
            Array.Copy(signature, 0, header, 12, signature.Length);

            return header;
        }

        private ulong getWindowsTimestamp()
        {
            var unixTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ulong windowsTimestamp = (unixTimestamp + 11644473600u) * 10000000u;
            return windowsTimestamp;
        }

        private string base64url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd(new char[] { '=' }).Replace('+', '-').Replace('/', '_');
        }
    }
}
