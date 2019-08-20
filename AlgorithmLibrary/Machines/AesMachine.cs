using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AlgorithmLibrary
{
    /// <summary>
    /// Defines the <see cref="AesMachine" /> which contains data and methods required for encrypting and decrypting blocks of data with AES algorithm.
    /// </summary>
    public class AesMachine : IMachine
    {
        public static readonly string Signature = "aes";

        /// <summary>
        /// Initializes a new instance of the <see cref="AesMachine"/> class with random Key and IV.
        /// </summary>
        public AesMachine()
        {
            this.Key = new byte[32];
            new Random().NextBytes(this.Key);
            this.IV = new byte[16];
            new Random().NextBytes(this.IV); // get random init vector
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AesMachine"/> class.
        /// </summary>
        /// <param name="key">32-byte key.<see cref="byte[]"/></param>
        /// <param name="iv">16-byte initialization vector.<see cref="byte[]"/></param>
        public AesMachine(byte[] key, byte[] iv)
        {
            this.Key = key;
            this.IV = iv;
        }

        public byte[] Key { get; set; }

        public byte[] IV { get; set; }

        public int BlockSize { get; } = 16384;

        public byte[] AdditionalData { get => this.IV; }

        public byte[] Encrypt(byte[] content)
        {
            using (AesManaged aes = new AesManaged())
            using (var encryptor = aes.CreateEncryptor(this.Key, this.IV))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                writer.Write(content, 0, content.Length);
                writer.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        public byte[] Decrypt(byte[] content)
        {
            using (AesManaged aes = new AesManaged())
            using (var decryptor = aes.CreateDecryptor(this.Key, this.IV))
            using (MemoryStream ms = new MemoryStream(content))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                var decrypted = new byte[content.Length];
                var bytesRead = cs.Read(decrypted, 0, content.Length);

                return decrypted.Take(bytesRead).ToArray();
            }
        }

        public string GetSignatureString()
        {
            return Signature;
        }
    }
}