using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// Defines the <see cref="UserInformation" />
    /// </summary>
    public class UserInformation
    {
        private readonly User user;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInformation"/> class.
        /// </summary>
        /// <param name="user"><see cref="User"/> instance that contains information from the database.</param>
        /// <param name="certificate">Certificate corresponding to <paramref name="user"/>.</param>
        /// <param name="privateKey"><see cref="RSAParameters"/> that contain private key of the <paramref name="user"/>.</param>
        public UserInformation(User user, RSAParameters privateKey)
        {
            this.user = user;
            this.PrivateKey = privateKey;
        }

        public RSAParameters PrivateKey { get; }

        public string Username { get => this.user.Username; }

        public X509Certificate2 Certificate { get => new X509Certificate2(this.user.PublicCertificate); }

        public RSAParameters PublicKey { get => ((RSACryptoServiceProvider)this.Certificate.PublicKey.Key).ExportParameters(false); }
    }
}