using Certificates;

using System.IO;

using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// Defines the <see cref="DataComponents"/> which is used for accessing <see cref="User"/> database and <see cref="CertificateManager"/>.
    /// </summary>
    public class DataComponents
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataComponents" /> class.
        /// </summary>
        /// <param name="userDbPath">Path to <see cref="User"/> database file.</param>
        /// <param name="certificateFolderPath">Path to folder with certificates.</param>
        public DataComponents(string userDbPath, string certificateFolderPath, string authorityPath)
        {
            UserDatabase = new UserDatabase(userDbPath);
            CertificateManager = new CertificateManager(certificateFolderPath, authorityPath);
        }

        public CertificateManager CertificateManager { get; }

        public UserDatabase UserDatabase { get; }
    }
}