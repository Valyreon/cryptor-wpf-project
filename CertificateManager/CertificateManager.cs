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
        public CertificateManager(string certFolderPath)
        {
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

        public void ReloadCertificates()
        {
            Certificates.Clear();
            string[] files = Directory.GetFiles(this.CertificatesFolderPath).ToArray();
            foreach (string cert in files)
            {
                try
                {
                    X509Certificate2 certificate = new X509Certificate2(cert);
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
            using (X509Chain chain = new X509Chain())
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = DateTime.Now;

                bool isChainValid = chain.Build(certificateToValidate);

                if (!isChainValid)
                {
                    string[] errors = chain.ChainStatus
                        .Select(x => String.Format("{0} ({1})", x.StatusInformation.Trim(), x.Status)) //TODO: Do you need this?
                        .ToArray();
                    string certificateErrorsString = "Unknown errors.";

                    if (errors != null && errors.Length > 0)
                    {
                        certificateErrorsString = string.Join(", ", errors); 
                    }

                    return false;
                }

                return true;
            }
        }

        public bool VerifyKeyUsage(X509Certificate2 cert)
        {
            List<X509KeyUsageExtension> extensions = cert.Extensions.OfType<X509KeyUsageExtension>().ToList();
            if (!extensions.Any())
            {
                return cert.Version < 3;
            }

            List<X509KeyUsageFlags> keyUsageFlags = extensions.Select((ext) => ext.KeyUsages).ToList();
            return keyUsageFlags.Contains(X509KeyUsageFlags.KeyEncipherment) && keyUsageFlags.Contains(X509KeyUsageFlags.DigitalSignature);
        }
    }
}