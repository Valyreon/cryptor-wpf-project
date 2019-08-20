using AlgorithmLibrary;
using CryptedStreamParsers;
using CryptedStreamParsers.Cryptors;
using System.IO;
using System.Security.Cryptography;

namespace FileEncryptorWpf.Models
{
    public class Encryptor
    {
        private readonly DataComponents dataSource;
        private readonly string receiverUsername;
        private readonly UserInformation currentUser;
        private readonly CryptCombo combo;

        public Encryptor(DataComponents dataSource, string receiverUsername, UserInformation currentUser, CryptCombo combo)
        {
            this.dataSource = dataSource;
            this.receiverUsername = receiverUsername;
            this.currentUser = currentUser;
            this.combo = combo;
        }

        public void EncryptFile(OriginalFile input, FileStream output, ProgressReporter reporter = null)
        {
            reporter?.Log("Getting receiver information from database...");
            var receiver = this.dataSource.UserDatabase.GetUser(this.receiverUsername);
            reporter?.SetPercentage(15);
            if (receiver == null)
            {
                reporter?.Log($"User with username '{this.receiverUsername}' was not found. Aborting.");
            }
            else
            {
                reporter?.Log("User found. Searching for user certificate...");
                var cert = this.dataSource.CertificateManager.GetCertificate(receiver.CertificateThumbprint);
                if (cert == null)
                {
                    reporter?.Log("Certificate was not found. Aborting.");
                }

                else
                {
                    reporter?.Log("Certificate located. Verifying certificate...");

                    if (this.dataSource.CertificateManager.VerifyCertificate(cert) == false)
                    {
                        reporter?.Log("Receiver's certificate is not signed by required CA or is invalid. Aborting.");
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
