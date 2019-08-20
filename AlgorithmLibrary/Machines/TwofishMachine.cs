using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

using System;

namespace AlgorithmLibrary
{
    /// <summary>
    /// Defines the <see cref="TwofishMachine"/> which contains data and methods required for encrypting and decrypting blocks of data with Twofish algorithm.
    /// </summary>
    public class TwofishMachine : IMachine
    {
        public static readonly string Signature = "tfh";

        /// <summary>
        /// Initializes a new instance of the <see cref="TwofishMachine"/> class with random Key and Salt.
        /// </summary>
        public TwofishMachine()
        {
            this.Key = new byte[32];
            new Random().NextBytes(this.Key);
            this.Salt = new byte[16];
            new Random().NextBytes(this.Salt); // get random salt vector
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwofishMachine"/> class.
        /// </summary>
        /// <param name="key">32-byte key<see cref="byte[]"/></param>
        /// <param name="salt">The salt<see cref="byte[]"/></param>
        public TwofishMachine(byte[] key, byte[] salt)
        {
            this.Key = key;
            this.Salt = salt;
        }

        public byte[] Key { get; set; }

        public byte[] Salt { get; set; }

        public int BlockSize { get; } = 16384;

        public byte[] AdditionalData { get => this.Salt; }

        public byte[] Decrypt(byte[] content)
        {
            Sha3Digest sha3Digest = new Sha3Digest();
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(sha3Digest);
            gen.Init(this.Key, this.Salt, 1000);
            KeyParameter param = (KeyParameter)gen.GenerateDerivedParameters(new TwofishEngine().AlgorithmName, 256);

            var blockCipher = new TwofishEngine();
            var padding = new Pkcs7Padding();
            try
            {
                var cipher = padding == null ?
                new PaddedBufferedBlockCipher(blockCipher) : new PaddedBufferedBlockCipher(blockCipher, padding);
                cipher.Init(false, param);
                return cipher.DoFinal(content);
            }
            catch (CryptoException)
            {
                // log exception
            }

            return null;
        }

        public byte[] Encrypt(byte[] content)
        {
            Sha3Digest sha3Digest = new Sha3Digest();
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(sha3Digest);
            gen.Init(this.Key, this.Salt, 1000);
            KeyParameter param = (KeyParameter)gen.GenerateDerivedParameters(new TwofishEngine().AlgorithmName, 256);
            var blockCipher = new TwofishEngine();
            var padding = new Pkcs7Padding();

            try
            {
                var cipher = padding == null ?
                new PaddedBufferedBlockCipher(blockCipher) : new PaddedBufferedBlockCipher(blockCipher, padding);
                cipher.Init(true, param);
                return cipher.DoFinal(content);
            }
            catch (CryptoException)
            {
                // log exception
            }

            return null;
        }

        public string GetSignatureString()
        {
            return Signature;
        }
    }
}