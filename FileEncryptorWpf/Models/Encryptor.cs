using AlgorithmLibrary;
using CryptedStreamParsers;
using CryptedStreamParsers.Cryptors;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    public class Encryptor
    {
        private readonly UserDatabase dataSource;
        private readonly string receiverUsername;
        private readonly UserInformation currentUser;
        private readonly CryptCombo combo;

        public Encryptor(UserDatabase dataSource, string receiverUsername, UserInformation currentUser, CryptCombo combo)
        {
            this.dataSource = dataSource;
            this.receiverUsername = receiverUsername;
            this.currentUser = currentUser;
            this.combo = combo;
        }

        public void EncryptFile(OriginalFile input, FileStream output, ProgressReporter reporter = null)
        {
            reporter?.Log("Getting receiver information from database...");
            var receiver = this.dataSource.GetUser(this.receiverUsername);
            reporter?.SetPercentage(15);
            if (receiver == null)
            {
                reporter?.Log($"User with username '{this.receiverUsername}' was not found. Aborting.");
            }
            else
            {
                reporter?.Log("User found. Getting user certificate...");
                var cert = new X509Certificate2(receiver.PublicCertificate);

                if (cert == null)
                {
                    reporter?.Log("Certificate error. Aborting.");
                }
                else
                {
                    reporter?.Log("Verifying certificate...");

                    if (CertificateValidator.VerifyCertificate(cert) is false)
                    {
                        reporter?.Log("Receiver's certificate is invalid. Aborting.");
                    }
                    else
                    {
                        reporter?.Log("Receiver's certificate is valid.");
                        reporter?.Log("Encrypting file...");
                        FileCryptor cryptor = new FileCryptor(this.currentUser.PrivateKey, ((RSACryptoServiceProvider)cert.PublicKey.Key).ExportParameters(false));
                        cryptor.Encrypt(input, output, combo, reporter.SetPercentage);
                        reporter?.Log("File encryption complete.");
                    }
                }
            }
        }
    }
}
