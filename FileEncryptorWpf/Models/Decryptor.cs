using AlgorithmLibrary;
using CryptedStreamParsers;
using System.IO;
using System.Security.Cryptography;

namespace FileEncryptorWpf.Models
{
    public class Decryptor
    {
        private readonly DataComponents dataSource;
        private readonly string senderUsername;
        private readonly UserInformation currentUser;

        public Decryptor(DataComponents dataSource, string senderUsername, UserInformation currentUser)
        {
            this.dataSource = dataSource;
            this.senderUsername = senderUsername;
            this.currentUser = currentUser;
        }

        public void DecryptFile(EncryptedFile input, FileStream output, ProgressReporter reporter = null)
        {
            reporter?.Log("Parsing encrypted input file...");

            reporter?.Log("Getting sender information from database...");
            var senderUser = this.dataSource.UserDatabase.GetUser(this.senderUsername);
            reporter?.SetPercentage(10);

            if (senderUser == null)
            {
                reporter?.Log((string.IsNullOrWhiteSpace(this.senderUsername) ? "No username provided. " : "User with specified ID is not in the database. ") + "Unable to verify integrity.");
            }
            else
            {
                reporter?.Log("User found. Searching for user certificate...");
                var cert = this.dataSource.CertificateManager.GetCertificate(senderUser.CertificateThumbprint);


                if (cert == null)
                {
                    reporter?.Log("Certificate was not located. Unable to verify integrity.");
                }
                else
                {
                    reporter?.Log("Certificate located.");
                    if (this.dataSource.CertificateManager.VerifyCertificate(cert) == false)
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
            }

            reporter?.Log("Decrypting file...");
            FileDecryptor decryptor = new FileDecryptor(this.currentUser.PrivateKey);
            decryptor.Decrypt(input, output, reporter.SetPercentage);
            reporter?.Log("File decryption complete.");


        }
    }
}
