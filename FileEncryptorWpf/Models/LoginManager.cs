using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AlgorithmLibrary;
using PrivateKeyParsers;
using UserDatabaseManager;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// Defines the <see cref="LoginManager" /> class that is used as a Model for LoginControl. It manages login in the WPF app.
    /// </summary>
    public class LoginManager
    {
        private readonly string privateKeyPath;
        private readonly string userDatabasePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginManager"/> class.
        /// </summary>
        /// <param name="privKey">Path to users private key.</param>
        /// <param name="userDb">Path to user database file.</param>
        public LoginManager(string privKey, string userDb)
        {
            this.privateKeyPath = privKey;
            this.userDatabasePath = userDb;
        }

        public UserInformation Login(string username, string password, out UserDatabase data)
        {
            UserDatabase dataComp = new UserDatabase(this.userDatabasePath);

            var user = dataComp.GetUser(username);

            if (user != null && user.IsPasswordValid(password))
            {
                var userCert = new X509Certificate2(user.PublicCertificate);

                if (userCert == null)
                {
                    throw new Exception("Certificate error.");
                }

                if (CertificateValidator.VerifyCertificate(userCert) == false)
                {
                    throw new Exception("Certificate is invalid.");
                }

                byte[] keyRaw = File.ReadAllBytes(this.privateKeyPath);
                var privateParameters = new KeyFileParser(keyRaw).GetParameters();
                RSACryptoServiceProvider publicKeyProvider = (RSACryptoServiceProvider)userCert.PublicKey.Key;
                if (!RsaMachine.AreKeysMatched(publicKeyProvider.ExportParameters(false), privateParameters))
                {
                    throw new Exception("The given private key does not match this user's certificate.");
                }

                data = dataComp;
                return new UserInformation(user, privateParameters);
            }
            else
            {
                throw new Exception("Invalid username or password.");
            }
        }
    }
}