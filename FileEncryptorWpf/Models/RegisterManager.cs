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
            /*if(this.Username.Length<6)
            {
                throw new Exception("Username must be at least 6 characters long.");
            }
            else if(!this.IsExternal && password.Length<8)
            {
                throw new Exception("password must be at least 8 characters long.");
            }
            else if(!File.Exists(this.CertificateFilePath))
            {
                throw new Exception("That certificate file does not exist.");
            }*/

            X509Certificate2 cert = new X509Certificate2(certificateFilePath);

            if (data.CertificateManager.VerifyCertificate(cert) == false)
            {
                throw new Exception("Certificate is either not signed by the required CA or is invalid.");
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
