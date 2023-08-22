using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// Defines the <see cref="CertificateValidator"/> class which loads and manages certificates from a directory.
    /// </summary>
    public static class CertificateValidator
    {
        public static bool VerifyCertificate(X509Certificate2 certificateToValidate)
        {
            using X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.VerificationTime = DateTime.Now;

            bool isChainValid = chain.Build(certificateToValidate);

            if (!isChainValid)
            {
                return false;
            }

            return true;
        }

        public static bool VerifyKeyUsage(X509Certificate2 cert)
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