using AlgorithmLibrary;
using PrivateKeyParsers;
using System;
using System.IO;
using System.Security.Cryptography;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// Defines the <see cref="LoginManager" /> class that is used as a Model for LoginControl. It manages login in the WPF app.
    /// </summary>
    public class LoginManager
    {
        private readonly string certificationsFolderPath;
        private readonly string privateKeyPath;
        private readonly string userDatabasePath;
        private readonly string authorityCertPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginManager"/> class.
        /// </summary>
        public LoginManager(string certs, string privKey, string userDb, string caPath)
        {
            certificationsFolderPath = certs;
            privateKeyPath = privKey;
            userDatabasePath = userDb;
            authorityCertPath = caPath;
        }

        public UserInformation Login(string username, string password, out DataComponents data)
        {
            DataComponents dataComp = new DataComponents(userDatabasePath, certificationsFolderPath, authorityCertPath);

            var user = dataComp.UserDatabase.GetUser(username);

            if (user != null && user.IsPasswordValid(password))
            {
                var userCert = dataComp.CertificateManager.GetCertificate(user.CertificateThumbprint);

                if(userCert==null)
                {
                    throw new Exception("That users certificate could not be found.");
                }

                if (dataComp.CertificateManager.VerifyCertificate(userCert) == false)
                {
                    throw new Exception("Certificate is either not signed by the required CA or is invalid.");
                }

                byte[] keyRaw = File.ReadAllBytes(privateKeyPath);
                var privateParameters = new KeyFileParser(keyRaw).GetParameters();
                RSACryptoServiceProvider publicKeyProvider = (RSACryptoServiceProvider)userCert.PublicKey.Key;
                if (!RsaMachine.AreKeysMatched(publicKeyProvider.ExportParameters(false), privateParameters))
                {
                    throw new Exception("The given private key does not match this user's certificate.");
                }

                data = dataComp;
                return new UserInformation(user, userCert, privateParameters);
            }
            else
            {
                throw new Exception("Invalid username or password.");
            }
        }
    }
}