using System.Security.Cryptography;

namespace FileEncryptorWpf.Models.ComboBoxObjects
{
    /// <summary>
    /// Defines the <see cref="HashAlgorithmChoice" />
    /// </summary>
    public class HashAlgorithmChoice
    {
        private static int nextIndex = 0;

        public HashAlgorithmChoice(string name, HashAlgorithm hashMachine)
        {
            this.Id = nextIndex++;
            this.Name = name;
            this.HashMachine = hashMachine;
        }

        public int Id { get; set; }

        public string Name { get; }

        public HashAlgorithm HashMachine { get; }
    }
}