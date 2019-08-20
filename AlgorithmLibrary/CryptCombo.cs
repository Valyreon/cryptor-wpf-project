using System.Security.Cryptography;

namespace AlgorithmLibrary
{
    /// <summary>
    /// Defines the <see cref="CryptCombo" /> class which contains a combination of an Encryption and a Hash algorithm.
    /// </summary>
    public class CryptCombo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CryptCombo"/> class.
        /// </summary>
        /// <param name="hash">The hash<see cref="HashAlgorithm"/></param>
        /// <param name="machine">The machine<see cref="IMachine"/></param>
        public CryptCombo(HashAlgorithm hash, IMachine machine)
        {
            this.Hasher = hash;
            this.Machine = machine;
        }

        public HashAlgorithm Hasher { get; set; }

        public IMachine Machine { get; set; }
    }
}