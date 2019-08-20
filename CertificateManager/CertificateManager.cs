using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Certificates
{
    /// <summary>
    /// Defines the <see cref="CertificateManager"/> class which loads and manages certificates from a directory.
    /// </summary>
    public class CertificateManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateManager"/> class.
        /// </summary>
        /// <param name="certFolderPath">The folder where all the certification files are located.<see cref="string"/></param>
        public CertificateManager(string certFolderPath, string pathToAuthority)
        {
            Authority = new X509Certificate2(pathToAuthority); // authority has to be set before ReloadCertificates
            this.CertificatesFolderPath = certFolderPath;
            this.ReloadCertificates();

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = certFolderPath,
                Filter = "*.pem | *.der | *.crt | *.cert"
            };

            void OnCreated(object source, FileSystemEventArgs e)
            {
                this.ReloadCertificates();
            }

            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.EnableRaisingEvents = true;
        }

        public string CertificatesFolderPath { get; }

        private List<X509Certificate2> Certificates { get; } = new List<X509Certificate2>();

        public readonly X509Certificate2 Authority;

        public void ReloadCertificates()
        {
            Certificates.Clear();
            string[] files = Directory.GetFiles(this.CertificatesFolderPath).ToArray();
            foreach (string cert in files)
            {
                try
                {
                    X509Certificate2 certificate = new X509Certificate2(cert);
                    if(certificate.Thumbprint != Authority.Thumbprint)
                    {
                        this.Certificates.Add(certificate);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public X509Certificate2 GetCertificate(string thumbprint)
        {
            return this.Certificates.Where(c => c.Thumbprint == thumbprint).SingleOrDefault();
        }

        public X509Certificate2 GetCertificate(int index)
        {
            return this.Certificates[index];
        }

        public bool VerifyCertificate(X509Certificate2 certificateToValidate)
        {
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.VerificationTime = DateTime.Now;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

            // This part is very important. You're adding your known root here.
            // It doesn't have to be in the computer store at all. Neither certificates do.
            chain.ChainPolicy.ExtraStore.Add(Authority);

            bool isChainValid = chain.Build(certificateToValidate);

            if (!isChainValid)
            {
                string[] errors = chain.ChainStatus
                    .Select(x => String.Format("{0} ({1})", x.StatusInformation.Trim(), x.Status))
                    .ToArray();
                string certificateErrorsString = "Unknown errors.";

                if (errors != null && errors.Length > 0)
                {
                    certificateErrorsString = String.Join(", ", errors);
                }

                throw new Exception("Trust chain did not complete to the known authority anchor. Errors: " + certificateErrorsString);
            }

            // This piece makes sure it actually matches your known root
            var valid = chain.ChainElements
                .Cast<X509ChainElement>()
                .Any(x => x.Certificate.Thumbprint == Authority.Thumbprint);

            /*if (!valid)
            {
                throw new Exception("Trust chain did not complete to the known authority anchor. Thumbprints did not match.");
            }*/
            return valid;
        }
    }
}