using System.Security.Cryptography;

namespace FileEncryptorWpf.Models.ComboBoxObjects
{
    /// <summary>
    /// Defines the <see cref="HashAlgorithmChoice" />
    /// </summary>
    public class HashAlgorithmChoice
    {
        private static int _nextIndex = 0;

        public int Id { get; set; }

        public string Name { get; }

        public HashAlgorithm HashMachine { get; }

        public HashAlgorithmChoice(string name, HashAlgorithm hashMachine)
        {
            Id = _nextIndex++;
            Name = name;
            HashMachine = hashMachine;
        }
    }
}