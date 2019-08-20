using System.IO;

namespace CryptedStreamParsers
{
    /// <summary>
    /// Defines the <see cref="OriginalFile" />
    /// </summary>
    public class OriginalFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OriginalFile"/> class.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> containing a file that is to be encrypted.</param>
        /// <param name="ext">Extension of the file.<see cref="string"/></param>
        public OriginalFile(Stream stream, string ext)
        {
            this.FileContent = stream;
            this.Extension = ext;
        }

        /// <summary>
        /// Gets the extension of the file.
        /// </summary>
        public string Extension { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Stream"/> containing a file that is to be encrypted.
        /// </summary>
        public Stream FileContent { get; internal set; }
    }
}