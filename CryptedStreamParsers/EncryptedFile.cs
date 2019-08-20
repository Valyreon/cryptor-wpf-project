using System.IO;

namespace CryptedStreamParsers
{
    /// <summary>
    /// Defines the <see cref="EncryptedFile" /> class which contains all header information from an encrypted file and the stream of the file.
    /// </summary>
    public class EncryptedFile
    {
        internal readonly Stream BaseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedFile"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the encrypted file.</param>
        internal EncryptedFile(Stream stream)
        {
            stream.Position = 0;
            this.BaseStream = stream;
        }

        public byte[] DsaSignature { get; internal set; }

        public string CryptorCode { get; internal set; }

        public string HasherCode { get; internal set; }

        public string FormatExtension { get; internal set; }

        public long NumberOfBlocks { get; internal set; }

        public int HeaderLength { get => 28 + this.FormatExtension.Length + this.EncryptedKey.Length; }

        public byte[] EncryptedKey { get; internal set; }
    }
}