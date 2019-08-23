using AlgorithmLibrary;

namespace FileEncryptorWpf.Models.ComboBoxObjects
{
    /// <summary>
    /// Defines the <see cref="EncryptionAlgorithmChoice" />
    /// </summary>
    public class EncryptionAlgorithmChoice
    {
        private static int nextIndex = 0;

        public EncryptionAlgorithmChoice(string name, IMachine hashMachine)
        {
            this.Id = nextIndex++;
            this.Name = name;
            this.CryptMachine = hashMachine;
        }

        public int Id { get; set; }

        public string Name { get; }

        public IMachine CryptMachine { get; }
    }
}