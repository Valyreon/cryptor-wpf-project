using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AlgorithmLibrary
{
    /// <summary>
    /// Defines the <see cref="TDesMachine" /> which contains data and methods required for encrypting and decrypting blocks of data with Triple DES algorithm.
    /// </summary>
    public class TDesMachine : IMachine
    {
        public static readonly string Signature = "tds";

        /// <summary>
        /// Initializes a new instance of the <see cref="TDesMachine"/> class with random Key and IV.
        /// </summary>
        public TDesMachine()
        {
            this.Key = new byte[24];
            new Random().NextBytes(this.Key);
            this.IV = new byte[16];
            new Random().NextBytes(this.IV); // get random init vector
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDesMachine"/> class.
        /// </summary>
        /// <param name="key">24-byte key<see cref="byte[]"/></param>
        /// <param name="iv">16-byte iv<see cref="byte[]"/></param>
        public TDesMachine(byte[] key, byte[] iv)
        {
            this.Key = key;
            this.IV = iv;
        }

        public byte[] Key { get; set; }

        public byte[] IV { get; set; }

        public int BlockSize { get; } = 115;

        public byte[] AdditionalData { get => this.IV; }

        public byte[] Decrypt(byte[] content)
        {
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            using (var decryptor = tdes.CreateDecryptor(this.Key, this.IV))
            using (MemoryStream ms = new MemoryStream(content))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                var decrypted = new byte[content.Length];
                var bytesRead = cs.Read(decrypted, 0, content.Length);

                return decrypted.Take(bytesRead).ToArray();
            }
        }

        public byte[] Encrypt(byte[] content)
        {
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            using (ICryptoTransform encryptor = tdes.CreateEncryptor(this.Key, this.IV))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                writer.Write(content, 0, content.Length);
                writer.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        public string GetSignatureString()
        {
            return Signature;
        }
    }
}