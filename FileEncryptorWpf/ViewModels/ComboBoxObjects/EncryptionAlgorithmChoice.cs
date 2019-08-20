using AlgorithmLibrary;

namespace FileEncryptorWpf.Models.ComboBoxObjects
{
    /// <summary>
    /// Defines the <see cref="EncryptionAlgorithmChoice" />
    /// </summary>
    public class EncryptionAlgorithmChoice
    {
        private static int _nextIndex = 0;

        public int Id { get; set; }

        public string Name { get; }

        public IMachine CryptMachine { get; }

        public EncryptionAlgorithmChoice(string name, IMachine hashMachine)
        {
            Id = _nextIndex++;
            Name = name;
            CryptMachine = hashMachine;
        }
    }
}