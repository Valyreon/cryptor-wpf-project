using AlgorithmLibrary;

using CryptedStreamParsers.Exceptions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptedStreamParsers
{
    /// <summary>
    /// Defines the <see cref="EncryptedFileChecker" /> static class which is used for parsing encrypted files and instancing <see cref="EncryptedFile" /> objects.
    /// </summary>
    public static class EncryptedFileChecker
    {
        /// <summary>
        /// This method will parse the <see cref="Stream"/> of the encrypted file.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the encrypted file.</param>
        /// <returns><see cref="EncryptedFile"/> populated with all the header information required for decryption.</returns>
        public static EncryptedFile Parse(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            EncryptedFile file = new EncryptedFile(reader.BaseStream);

            reader.BaseStream.Position = 0;

            if (!reader.ReadBytes(6).SequenceEqual(Helpers.MagicBytes))
            {
                throw new InvalidFileException();
            }

            file.CryptorCode = Encoding.ASCII.GetString(reader.ReadBytes(Helpers.CryptorCodenameSize));
            if (!file.CryptorCode.Equals(AesMachine.Signature) && !file.CryptorCode.Equals(TDesMachine.Signature) && !file.CryptorCode.Equals(TwofishMachine.Signature))
            {
                throw new UnknownCryptorCodeException(file.CryptorCode);
            }

            file.HasherCode = Encoding.ASCII.GetString(reader.ReadBytes(Helpers.HasherCodenameSize));
            if (!Helpers.IsHasherCodeValid(file.HasherCode))
            {
                throw new UnknownHasherCodeException(file.HasherCode);
            }

            int cryptedKeyLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            file.EncryptedKey = reader.ReadBytes(cryptedKeyLength);

            int extLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            file.FormatExtension = Encoding.ASCII.GetString(reader.ReadBytes(extLength));

            file.NumberOfBlocks = BitConverter.ToInt64(reader.ReadBytes(8), 0);

            reader.BaseStream.Position += 16;
            for (int i = 0; i < file.NumberOfBlocks + 1; i++)
            {
                int blockSize = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                reader.BaseStream.Position += blockSize;
            }

            if (!(reader.BaseStream.Position == reader.BaseStream.Length))
            {
                throw new InvalidFileException();
            }

            return file;
        }

        /// <summary>
        /// This method will verify the signature of the encrypted file.
        /// </summary>
        /// <param name="file">The <see cref="EncryptedFile"/> whose signature will be verified.</param>
        /// <param name="param">Public key of the sender.</param>
        /// <returns>Boolean indicating if the signature is valid.</returns>
        public static bool VerifySignature(EncryptedFile file, RSAParameters param)
        {
            BinaryReader reader = new BinaryReader(file.BaseStream);
            return VerifyDsaSignature(file, reader, param);
        }

        private static bool VerifyDsaSignature(EncryptedFile file, BinaryReader reader, RSAParameters key)
        {
            var hasher = Helpers.GetHasherFromCode(file.HasherCode);
            reader.BaseStream.Position = 0;

            byte[] header = reader.ReadBytes(file.HeaderLength);
            var contentHashAggregate = hasher.ComputeHash(header);

            byte[] additionalData = reader.ReadBytes(16);
            contentHashAggregate = hasher.ComputeHash(contentHashAggregate.Concat(hasher.ComputeHash(additionalData)).ToArray());

            for (int i = 0; i < file.NumberOfBlocks; i++)
            {
                byte[] blockSizeArray = reader.ReadBytes(4);
                int blockSize = BitConverter.ToInt32(blockSizeArray, 0);
                byte[] block = reader.ReadBytes(blockSize);
                contentHashAggregate = hasher.ComputeHash(contentHashAggregate.Concat(hasher.ComputeHash(blockSizeArray.Concat(block).ToArray())).ToArray());
            }

            int rsaLength = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            byte[] rsaSignature = reader.ReadBytes(rsaLength);

            return new RsaMachine(key).CheckSignature(contentHashAggregate, hasher, rsaSignature);

        }
    }
}