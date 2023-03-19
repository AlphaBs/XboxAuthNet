using NUnit.Framework;
using System;
using System.Security.Cryptography;

namespace XboxAuthNet.Test
{
    [TestFixture]
    public class TestEC
    {
        [Test]
        public void GenerateKeyPair1()
        {
            using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                byte[] privateKey = ecdsa.ExportECPrivateKey();
                byte[] publicKey = ecdsa.ExportSubjectPublicKeyInfo();

                var parameters = ecdsa.ExportParameters(true);

                var privateKeyBytes = parameters.D;
                var publicKeyXBytes = parameters.Q.X;
                var publicKeyYBytes = parameters.Q.Y;

                Console.WriteLine("D " + BitConverter.ToString(privateKeyBytes!));
                Console.WriteLine("X " + BitConverter.ToString(publicKeyXBytes!));
                Console.WriteLine("Y " + BitConverter.ToString(publicKeyYBytes!));
            }

            /*
            D 9C-DF-F4-2B-AE-1F-C8-93-46-15-2E-ED-39-9D-06-CB-39-26-3C-1C-31-2B-68-B9-FF-8F-48-5C-43-B4-D1-42
            X 5A-1A-7F-E2-E3-EE-FA-F4-F8-B5-03-3E-DD-05-DF-4E-E5-00-12-A8-AC-68-CD-DA-31-F9-55-A4-B1-9E-4A-43
            Y 59-BE-F2-72-A2-11-72-57-58-AD-AA-DE-9E-8F-A5-B7-E8-B9-0A-6D-CA-B6-F2-CD-69-6A-2F-A5-21-3B-5B-E3
            */
        }

        [Test]
        [Platform("win")]
        public void GenerateKeyPairCNG()
        {
            // System.PlatformNotSupportedException : Windows Cryptography Next Generation (CNG) is not supported on this platform.

            CngKey cngKey;
            ECDiffieHellmanCng ecDhCng;

            cngKey = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256);
            ecDhCng = new ECDiffieHellmanCng(cngKey);

            var parameters = ecDhCng.ExportParameters(true);

            var privateKeyBytes = parameters.D;
            var publicKeyXBytes = parameters.Q.X;
            var publicKeyYBytes = parameters.Q.Y;

            Console.WriteLine("D " + BitConverter.ToString(privateKeyBytes!));
            Console.WriteLine("X " + BitConverter.ToString(publicKeyXBytes!));
            Console.WriteLine("Y " + BitConverter.ToString(publicKeyYBytes!));
        }
    }
}
