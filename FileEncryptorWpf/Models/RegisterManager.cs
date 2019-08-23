using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// An MVVM model class used for registering new users.
    /// </summary>
    public class RegisterManager
    {
        private readonly UserDatabase data;

        public RegisterManager(UserDatabase data)
        {
            this.data = data;
        }

        internal void Register(string username, string certificateFilePath, string password, bool isExt = false)
        {
            X509Certificate2 cert = new X509Certificate2(certificateFilePath);

            if (CertificateValidator.VerifyCertificate(cert) == false)
            {
                throw new Exception("Certificate is invalid.");
            }
            else if (CertificateValidator.VerifyKeyUsage(cert) == false)
            {
                throw new Exception("Certificate must have 'digitalSignature' and 'keyEncipherment' set as it's key usage.");
            }

            if (isExt)
            {
                this.data.AddExternal(username, File.ReadAllBytes(certificateFilePath));
            }
            else
            {
                this.data.AddUser(username, password, File.ReadAllBytes(certificateFilePath));
            }
        }
    }
}
