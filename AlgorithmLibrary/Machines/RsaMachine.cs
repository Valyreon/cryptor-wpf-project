using System;
using System.Linq;
using System.Security.Cryptography;

namespace AlgorithmLibrary
{
    /// <summary>
    /// Defines the <see cref="RsaMachine" /> class which contains data and methods required for encrypting and decrypting blocks of data with RSA algorithm.
    /// </summary>
    public class RsaMachine : IMachine
    {
        public static readonly string Signature = "rsa";

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaMachine"/> class.
        /// </summary>
        /// <param name="rsaKey">The RSA key public or private key<see cref="RSAParameters"/></param>
        public RsaMachine(RSAParameters rsaKey)
        {
            this.Key = rsaKey;
        }

        public RSAParameters Key { get; set; }

        public int BlockSize { get; } = 115;

        byte[] IMachine.Key => null;

        public byte[] AdditionalData { get => null; }

        public static bool AreKeysMatched(RSAParameters publicKey, RSAParameters privateKey)
        {
            byte[] data = new byte[10];
            new Random().NextBytes(data);

            using (RSACryptoServiceProvider decryptRSA = new RSACryptoServiceProvider())
            using (RSACryptoServiceProvider encryptRSA = new RSACryptoServiceProvider())
            {
                encryptRSA.ImportParameters(publicKey);
                decryptRSA.ImportParameters(privateKey);

                var decrypted = decryptRSA.Decrypt(encryptRSA.Encrypt(data, false), false);

                if (data.SequenceEqual(decrypted))
                {
                    return true;
                }

                return false;
            }
        }

        public byte[] Decrypt(byte[] content)
        {
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.ImportParameters(this.Key);
                decryptedData = rsaProvider.Decrypt(content, false);
            }

            return decryptedData;
        }

        public byte[] Encrypt(byte[] content)
        {
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.ImportParameters(this.Key);
                encryptedData = rsaProvider.Encrypt(content, false);
            }

            return encryptedData;
        }

        public byte[] Sign(byte[] data, HashAlgorithm hasher)
        {
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.ImportParameters(this.Key);
                return rsaProvider.SignData(data, hasher);
            }
        }

        public bool CheckSignature(byte[] data, HashAlgorithm hasher, byte[] signature)
        {
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.ImportParameters(this.Key);
                return rsaProvider.VerifyData(data, hasher, signature);
            }
        }

        public string GetSignatureString()
        {
            return Signature;
        }
    }
}