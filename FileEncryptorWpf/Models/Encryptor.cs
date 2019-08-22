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
        private readonly User receiver;
        private readonly UserInformation currentUser;
        private readonly CryptCombo combo;

        public Encryptor(User receiverUsername, UserInformation currentUser, CryptCombo combo)
        {
            this.receiver = receiverUsername;
            this.currentUser = currentUser;
            this.combo = combo;
        }

        public void EncryptFile(OriginalFile input, FileStream output, ProgressReporter reporter = null)
        {
            var cert = new X509Certificate2(receiver.PublicCertificate);

            if (cert == null)
            {
                reporter?.Log("Receiver certificate error. Aborting.");
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
