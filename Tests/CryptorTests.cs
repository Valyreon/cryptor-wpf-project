using AlgorithmLibrary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Security.Cryptography;

namespace Tests
{
    /// <summary>
    /// Defines the <see cref="CryptorTests" />
    /// </summary>
    [TestClass]
    public class CryptorTests
    {
        [TestMethod]
        public void AesCryptorTest()
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                byte[] data = new byte[100];
                new Random().NextBytes(data);

                AesMachine cryptor = new AesMachine();

                byte[] encrypted = cryptor.Encrypt(data);
                byte[] decrypted = cryptor.Decrypt(encrypted);

                Assert.IsTrue(data.SequenceEqual(decrypted));
            }
        }

        [TestMethod]
        public void TwofishCryptorTest()
        {
            byte[] data = new byte[100];
            new Random().NextBytes(data);

            var cryptor = new TwofishMachine();

            byte[] encrypted = cryptor.Encrypt(data);
            byte[] decrypted = cryptor.Decrypt(encrypted);

            Assert.IsTrue(data.SequenceEqual(decrypted));
        }

        [TestMethod]
        public void RsaCryptorTest()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            //string str = RSA.ToXmlString(true);
            RSAParameters RSAKeyInfo = RSA.ExportParameters(true);

            byte[] data = new byte[100];
            new Random().NextBytes(data);

            var cryptor = new RsaMachine(RSAKeyInfo);

            byte[] encrypted = cryptor.Encrypt(data);
            byte[] decrypted = cryptor.Decrypt(encrypted);

            Assert.IsTrue(data.SequenceEqual(decrypted));
        }

        [TestMethod]
        public void DsaSignerTest()
        {
            RSACryptoServiceProvider dsaprovider = new RSACryptoServiceProvider();
            RSAParameters keys = dsaprovider.ExportParameters(true);

            byte[] data = new byte[100];
            new Random().NextBytes(data);

            RsaMachine signer = new RsaMachine(keys);
            HashAlgorithm hasher = MD5.Create();

            byte[] signature = signer.Sign(data, hasher);

            Assert.IsTrue(signer.CheckSignature(data, hasher, signature));
        }
    }
}