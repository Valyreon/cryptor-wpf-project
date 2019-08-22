using FileEncryptorWpf.ViewModels;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    public class RegisterManager
    {
        private readonly DataComponents data;

        public RegisterManager(DataComponents data)
        {
            this.data = data;
        }

        internal void Register(string username, string certificateFilePath, string password, bool isExt = false)
        {
            X509Certificate2 cert = new X509Certificate2(certificateFilePath);

            if (data.CertificateValidator.VerifyCertificate(cert) == false)
            {
                throw new Exception("Certificate is either not signed by the required CA or is invalid.");
            }
            else if (data.CertificateValidator.VerifyKeyUsage(cert) == false)
            {
                throw new Exception("Certificate must have 'digitalSignature' and 'keyEncipherment' set as it's key usage.");
            }

            if (isExt)
            {
                this.data.UserDatabase.AddExternal(username, File.ReadAllBytes(certificateFilePath));
            }
            else
            {
                this.data.UserDatabase.AddUser(username, password, File.ReadAllBytes(certificateFilePath));
            }
        }
    }
}
