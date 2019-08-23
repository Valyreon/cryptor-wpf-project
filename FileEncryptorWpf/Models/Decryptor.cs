using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CryptedStreamParsers;
using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    public class Decryptor
    {
        private readonly User sender;
        private readonly UserInformation currentUser;

        public Decryptor(User sender, UserInformation currentUser)
        {
            this.sender = sender;
            this.currentUser = currentUser;
        }

        public void DecryptFile(EncryptedFile input, FileStream output, ProgressReporter reporter = null)
        {
            var cert = new X509Certificate2(this.sender.PublicCertificate);

            if (cert == null)
            {
                reporter?.Log("Sender certificate error. Unable to verify integrity.");
            }
            else
            {
                reporter?.Log("Certificate located.");
                if (CertificateValidator.VerifyCertificate(cert) == false)
                {
                    reporter?.Log("Sender's certificate is INVALID. Continuing.");
                }

                reporter?.Log("Verifying file integrity...");
                RSACryptoServiceProvider publicKeyProvider = (RSACryptoServiceProvider)cert.PublicKey.Key;
                bool verifySuccess = EncryptedFileChecker.VerifySignature(input, publicKeyProvider.ExportParameters(false));
                if (verifySuccess)
                {
                    reporter?.Log("File verification: SUCCESS");
                }
                else
                {
                    reporter?.Log("File verification: FAILED");
                }

                reporter?.SetPercentage(25);
            }

            reporter?.Log("Decrypting file...");
            FileDecryptor decryptor = new FileDecryptor(this.currentUser.PrivateKey);
            decryptor.Decrypt(input, output, reporter.SetPercentage);
            reporter?.Log("File decryption complete.");
        }
    }
}
