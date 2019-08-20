using AlgorithmLibrary;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptedStreamParsers.Cryptors
{
    /// <summary>
    /// Defines the <see cref="FileCryptor" /> class that is used for file encryption.
    /// </summary>
    public class FileCryptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCryptor"/> class.
        /// </summary>
        /// <param name="senderPrivateKey"><see cref="RSAParameters"/> with the private key of the sender used for signing the file.</param>
        /// <param name="receiverPublicKey"><see cref="RSAParameters"/> with the public key of the receiver used for encrypting the file.</param>
        public FileCryptor(RSAParameters senderPrivateKey, RSAParameters receiverPublicKey)
        {
            this.SenderPrivateKey = senderPrivateKey;
            this.ReceiverPublicKey = receiverPublicKey;
        }

        public RSAParameters SenderPrivateKey { get; set; }

        public RSAParameters ReceiverPublicKey { get; set; }

        /// <summary>
        /// Encrypts a file using a combination of an encryption and a hash algorithm.
        /// </summary>
        /// <param name="input"><see cref="OriginalFile"/> to be encrypted.</param>
        /// <param name="output">Output <see cref="Stream"/> where the encrypted file will be written.</param>
        /// <param name="algs">Combination of an encryption and a hash algorithm.</param>
        public void Encrypt(OriginalFile input, Stream output, CryptCombo algs, Action<int> reportProgress = null)
        {
            BinaryWriter writer = new BinaryWriter(output); // to write to output
            BinaryReader reader = new BinaryReader(input.FileContent);
            writer.Seek(0, SeekOrigin.Begin); // write to beginning, this will override if there is something there
            reader.BaseStream.Position = 0; // read from beginning

            //List<byte[]> hashList = new List<byte[]>();

            long numberOfBlocks = input.FileContent.Length / algs.Machine.BlockSize;
            if (numberOfBlocks * algs.Machine.BlockSize < input.FileContent.Length)
            {
                numberOfBlocks++;
            }

            var algKey = algs.Machine.Key;
            var cryptedkey = new RsaMachine(this.ReceiverPublicKey).Encrypt(algKey);

            byte[] header = new byte[] { };
            header = header // we concat here instead of writing directly so we can calculate hash
                .Concat(Helpers.MagicBytes)
                .Concat(Encoding.ASCII.GetBytes(algs.Machine.GetSignatureString()))
                .Concat(Encoding.ASCII.GetBytes(Helpers.GetCodeFromHasher(algs.Hasher)))
                .Concat(BitConverter.GetBytes(cryptedkey.Length))
                .Concat(cryptedkey)
                .Concat(BitConverter.GetBytes(input.Extension.Length))
                .Concat(Encoding.ASCII.GetBytes(input.Extension))
                .Concat(BitConverter.GetBytes(numberOfBlocks))
                .ToArray();

            var contentHashAggregate = algs.Hasher.ComputeHash(header);
            writer.Write(header);
            reportProgress?.Invoke(25);

            byte[] additionalData = algs.Machine.AdditionalData;
            if (additionalData != null)
            {
                contentHashAggregate = algs.Hasher.ComputeHash(contentHashAggregate.Concat(algs.Hasher.ComputeHash(additionalData)).ToArray());
                writer.Write(additionalData);
            }
            reportProgress?.Invoke(30);
            for (int i = 0; i < numberOfBlocks; i++)
            {
                byte[] buffer = reader.ReadBytes(algs.Machine.BlockSize);
                byte[] encryptedBlock = algs.Machine.Encrypt(buffer);
                byte[] encSize = BitConverter.GetBytes(encryptedBlock.Length);
                byte[] blockToWrite = encSize.Concat(encryptedBlock).ToArray();
                contentHashAggregate = algs.Hasher.ComputeHash(contentHashAggregate.Concat(algs.Hasher.ComputeHash(blockToWrite)).ToArray());
                writer.Write(blockToWrite);

                int progress = (int)(((float)i / (float)numberOfBlocks) * 1000.00);
                reportProgress?.Invoke(progress < 980 ? progress : 980);
            }

            byte[] rsaSignature = new RsaMachine(this.SenderPrivateKey).Sign(contentHashAggregate, algs.Hasher);
            reportProgress?.Invoke(990);

            writer.Write(rsaSignature.Length);
            writer.Write(rsaSignature);
            reportProgress?.Invoke(1000);
        }
    }
}