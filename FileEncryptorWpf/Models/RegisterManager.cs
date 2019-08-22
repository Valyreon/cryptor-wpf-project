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

        

        public void Register(string password)
        {
            
        }

        internal void Register(string username, string certificateFilePath, string password, bool isExt = false)
        {
            X509Certificate2 cert = new X509Certificate2(certificateFilePath);

            if (data.CertificateManager.VerifyCertificate(cert) == false)
            {
                throw new Exception("Certificate is either not signed by the required CA or is invalid.");
            }
            else if (data.CertificateManager.VerifyKeyUsage(cert) == false)
            {
                throw new Exception("Certificate must have 'digitalSignature' and 'keyEncipherment' set as it's key usage.");
            }

            if (isExt)
            {
                this.data.UserDatabase.AddExternal(username, cert.Thumbprint);
            }
            else
            {
                this.data.UserDatabase.AddUser(username, password, cert.Thumbprint);
            }
        }
    }
}
