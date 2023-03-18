using NUnit.Framework;
using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

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

                Console.WriteLine("D " + BitConverter.ToString(privateKeyBytes));
                Console.WriteLine("X " + BitConverter.ToString(publicKeyXBytes));
                Console.WriteLine("Y " + BitConverter.ToString(publicKeyYBytes));
            }

            /*
            D 9C-DF-F4-2B-AE-1F-C8-93-46-15-2E-ED-39-9D-06-CB-39-26-3C-1C-31-2B-68-B9-FF-8F-48-5C-43-B4-D1-42
            X 5A-1A-7F-E2-E3-EE-FA-F4-F8-B5-03-3E-DD-05-DF-4E-E5-00-12-A8-AC-68-CD-DA-31-F9-55-A4-B1-9E-4A-43
            Y 59-BE-F2-72-A2-11-72-57-58-AD-AA-DE-9E-8F-A5-B7-E8-B9-0A-6D-CA-B6-F2-CD-69-6A-2F-A5-21-3B-5B-E3
            */
        }

        [Test]
        public void GenerateKeyPair2()
        {
            var random = new SecureRandom();

            var curve = ECNamedCurveTable.GetByName("prime256v1");
            var ec = new ECKeyPairGenerator();
            var ecrandom = new ECKeyGenerationParameters(new ECDomainParameters(curve), random);
            ec.Init(ecrandom);

            var keypair = ec.GenerateKeyPair();
            var privateKey = (ECPrivateKeyParameters)keypair.Private;
            var publicKey = (ECPublicKeyParameters)keypair.Public;

            var privateKeyBytes1 = privateKey.D.ToByteArray();
            var publicKeyXBytes1 = publicKey.Q.AffineXCoord.ToBigInteger().ToByteArray();
            var publicKeyYBytes1 = publicKey.Q.AffineYCoord.ToBigInteger().ToByteArray();

            var publicKeyXBytes2 = publicKey.Q.XCoord.ToBigInteger().ToByteArray();
            var publicKeyYBytes2 = publicKey.Q.YCoord.ToBigInteger().ToByteArray();

            Console.WriteLine("X1 " + BitConverter.ToString(publicKeyXBytes1));
            Console.WriteLine("Y1 " + BitConverter.ToString(publicKeyYBytes1));
            Console.WriteLine("X2 " + BitConverter.ToString(publicKeyXBytes2));
            Console.WriteLine("Y2 " + BitConverter.ToString(publicKeyYBytes2));

            Assert.AreEqual(publicKeyXBytes1, publicKeyXBytes2);
            Assert.AreEqual(publicKeyYBytes1, publicKeyYBytes2);

            //Console.WriteLine("D " + BitConverter.ToString(privateKeyBytes));
            //Console.WriteLine("X " + BitConverter.ToString(publicKeyXBytes));
            //Console.WriteLine("Y " + BitConverter.ToString(publicKeyYBytes));

            /*
            D 6D-53-D6-82-02-06-CE-75-BE-9A-6B-7C-68-5F-06-7C-F1-67-88-EF-A3-63-06-48-DA-F4-9C-4E-B6-24-FE-12
            X 00-A4-69-3C-0A-1A-48-06-EA-6B-59-26-D8-26-BB-D7-BA-7D-13-67-95-3C-34-DA-EF-02-D2-B6-81-7E-18-09-A5
            Y 13-48-1B-26-F4-C9-8E-2A-B0-C5-7F-9A-AA-A9-00-29-2B-6E-7C-7F-7F-8C-6F-A2-74-9D-46-CE-4A-E1-EA-91
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

            Console.WriteLine("D " + BitConverter.ToString(privateKeyBytes));
            Console.WriteLine("X " + BitConverter.ToString(publicKeyXBytes));
            Console.WriteLine("Y " + BitConverter.ToString(publicKeyYBytes));
        }
    }
}
