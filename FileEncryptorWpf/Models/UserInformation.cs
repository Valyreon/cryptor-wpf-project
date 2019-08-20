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
        private User user;

        private X509Certificate2 certificate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInformation"/> class.
        /// </summary>
        /// <param name="user"><see cref="User"/> instance that contains information from the database.</param>
        /// <param name="certificate">Certificate corresponding to <paramref name="user"/>.</param>
        /// <param name="privateKey"><see cref="RSAParameters"/> that contain private key of the <paramref name="user"/>.</param>
        public UserInformation(User user, X509Certificate2 certificate, RSAParameters privateKey)
        {
            this.user = user;
            this.certificate = certificate;
            this.PrivateKey = privateKey;
        }

        public RSAParameters PrivateKey { get; }

        public string Username { get => this.user.Username; }

        public string CertificateThumbprint { get => this.user.CertificateThumbprint; }

        public RSAParameters PublicKey { get => ((RSACryptoServiceProvider)this.certificate.PublicKey.Key).ExportParameters(false); }
    }
}