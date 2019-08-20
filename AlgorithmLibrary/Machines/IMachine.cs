namespace AlgorithmLibrary
{
    /// <summary>
    /// Defines the <see cref="IMachine" /> interface for implementation by all encryption algorithm machines.
    /// </summary>
    public interface IMachine
    {
        int BlockSize { get; }

        byte[] Key { get; }

        byte[] AdditionalData { get; }

        byte[] Encrypt(byte[] content);

        byte[] Decrypt(byte[] content);

        string GetSignatureString();
    }
}