using System;
using System.IO;
using System.Security.Cryptography;
using AlgorithmLibrary;

namespace CryptedStreamParsers
{
    /// <summary>
    /// Defines the <see cref="FileDecryptor" /> class that is used for file decryption.
    /// </summary>
    public class FileDecryptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDecryptor"/> class.
        /// </summary>
        /// <param name="privateKey"><see cref="RSAParameters"/> with the private key of the receiver.</param>
        public FileDecryptor(RSAParameters privateKey)
        {
            this.Key = privateKey;
        }

        public RSAParameters Key { get; set; }

        /// <summary>
        /// Decrypts a file.
        /// </summary>
        /// <param name="input"><see cref="EncryptedFile"/> that will be decrypted.</param>
        /// <param name="output">Output <see cref="Stream"/> where the decrypted file will be written.</param>
        /// <param name="reportProgress">Action that is used to report progress of decryption in percentages.</param>
        public void Decrypt(EncryptedFile input, Stream output, Action<int> reportProgress = null)
        {
            BinaryReader reader = new BinaryReader(input.BaseStream);
            reader.BaseStream.Position = input.HeaderLength;
            output.Position = 0;
            BinaryWriter outputWriter = new BinaryWriter(output);

            byte[] decryptedAlgKey = new RsaMachine(this.Key).Decrypt(input.EncryptedKey);
            reportProgress?.Invoke(35);

            IMachine decryptor = null;
            if (input.CryptorCode == AesMachine.Signature)
            {
                decryptor = new AesMachine(decryptedAlgKey, reader.ReadBytes(16));
            }
            else if (input.CryptorCode == TDesMachine.Signature)
            {
                decryptor = new TDesMachine(decryptedAlgKey, reader.ReadBytes(16));
            }
            else if (input.CryptorCode == TwofishMachine.Signature)
            {
                decryptor = new TwofishMachine(decryptedAlgKey, reader.ReadBytes(16));
            }

            for (int i = 0; i < input.NumberOfBlocks; i++)
            {
                int currentBlockSize = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                byte[] block = reader.ReadBytes(currentBlockSize);
                outputWriter.Write(decryptor.Decrypt(block));
                reportProgress?.Invoke((int)(((float)i / (float)input.NumberOfBlocks) * 1000.00));
            }

            reportProgress?.Invoke(1000);
        }
    }
}