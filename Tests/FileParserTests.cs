using AlgorithmLibrary;

using CryptedStreamParsers;
using CryptedStreamParsers.Cryptors;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Tests
{
    /// <summary>
    /// Defines the <see cref="FileParserTests" />
    /// </summary>
    [TestClass]
    public class FileParserTests
    {
        [TestMethod]
        public void TestAesFileDecryption()
        {
            byte[] data = new byte[10000];
            new Random().NextBytes(data); // fill random data

            byte[] key = new byte[32];
            new Random().NextBytes(key);

            var senderRsa = new RSACryptoServiceProvider();
            var senderPrivateKey = senderRsa.ExportParameters(true);
            //var senderPublicKey = senderRsa.ExportParameters(false);

            var receiverRsa = new RSACryptoServiceProvider();
            var receiverPrivateKey = receiverRsa.ExportParameters(true);
            var receiverPublicKey = receiverRsa.ExportParameters(false);

            CryptCombo combo = new CryptCombo(MD5.Create(), new AesMachine());
            MemoryStream cryptedFileStream = new MemoryStream();

            OriginalFile originFile = new OriginalFile(new MemoryStream(data), ".java");
            FileCryptor cryptor = new FileCryptor(senderPrivateKey, receiverPublicKey);
            cryptor.Encrypt(originFile, cryptedFileStream, combo);

            MemoryStream decryptedStream = new MemoryStream();
            EncryptedFile newCryptedFile = EncryptedFileChecker.Parse(cryptedFileStream);
            FileDecryptor decryptor = new FileDecryptor(receiverPrivateKey);
            decryptor.Decrypt(newCryptedFile, decryptedStream);

            Assert.IsTrue(decryptedStream.ToArray().SequenceEqual(data));
        }

        [TestMethod]
        public void TestTripleDesFileDecryption()
        {
            byte[] data = new byte[1000];
            new Random().NextBytes(data); // fill random data

            var senderRsa = new RSACryptoServiceProvider();
            var senderPrivateKey = senderRsa.ExportParameters(true);
            //var senderPublicKey = senderRsa.ExportParameters(false);

            var receiverRsa = new RSACryptoServiceProvider();
            var receiverPrivateKey = receiverRsa.ExportParameters(true);
            var receiverPublicKey = receiverRsa.ExportParameters(false);

            CryptCombo combo = new CryptCombo(MD5.Create(), new TDesMachine());
            MemoryStream cryptedFileStream = new MemoryStream();

            OriginalFile originFile = new OriginalFile(new MemoryStream(data), ".java");
            FileCryptor cryptor = new FileCryptor(senderPrivateKey, receiverPublicKey);
            cryptor.Encrypt(originFile, cryptedFileStream, combo);

            MemoryStream decryptedStream = new MemoryStream();
            EncryptedFile newCryptedFile = EncryptedFileChecker.Parse(cryptedFileStream);
            FileDecryptor decryptor = new FileDecryptor(receiverPrivateKey);
            decryptor.Decrypt(newCryptedFile, decryptedStream);

            Assert.IsTrue(decryptedStream.ToArray().SequenceEqual(data));
        }

        [TestMethod]
        public void TestTwofishFileDecryption()
        {
            byte[] data = new byte[10000];
            new Random().NextBytes(data); // fill random data

            var senderRsa = new RSACryptoServiceProvider();
            var senderPrivateKey = senderRsa.ExportParameters(true);
            //var senderPublicKey = senderRsa.ExportParameters(false);

            var receiverRsa = new RSACryptoServiceProvider();
            var receiverPrivateKey = receiverRsa.ExportParameters(true);
            var receiverPublicKey = receiverRsa.ExportParameters(false);

            CryptCombo combo = new CryptCombo(MD5.Create(), new TwofishMachine());
            MemoryStream cryptedFileStream = new MemoryStream();

            OriginalFile originFile = new OriginalFile(new MemoryStream(data), ".java");
            FileCryptor cryptor = new FileCryptor(senderPrivateKey, receiverPublicKey);
            cryptor.Encrypt(originFile, cryptedFileStream, combo);

            MemoryStream decryptedStream = new MemoryStream();
            EncryptedFile newCryptedFile = EncryptedFileChecker.Parse(cryptedFileStream);
            FileDecryptor decryptor = new FileDecryptor(receiverPrivateKey);
            decryptor.Decrypt(newCryptedFile, decryptedStream);

            Assert.IsTrue(decryptedStream.ToArray().SequenceEqual(data));
        }
    }
}