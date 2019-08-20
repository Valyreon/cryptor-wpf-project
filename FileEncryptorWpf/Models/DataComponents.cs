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
            if (File.Exists(userDbPath))
            {
                UserDatabase = new UserDatabase(userDbPath);
            }
            else
            {
                throw new FileNotFoundException("User database file was not found.");
            }

            if (!File.Exists(authorityPath))
            {
                throw new FileNotFoundException("Invalid CA certificate path.");
            }

            if (Directory.Exists(certificateFolderPath))
            {
                CertificateManager = new CertificateManager(certificateFolderPath, authorityPath);
            }
            else
            {
                throw new FileNotFoundException("Certificate folder was not found.");
            }
        }

        public CertificateManager CertificateManager { get; }

        public UserDatabase UserDatabase { get; }
    }
}