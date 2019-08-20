using CryptedStreamParsers.Exceptions;

using System.Security.Cryptography;

namespace CryptedStreamParsers
{
    /// <summary>
    /// Defines the <see cref="Helpers" /> that contains some constants and helper methods.
    /// </summary>
    internal static class Helpers
    {
        public static readonly byte[] MagicBytes = new byte[] { 96, 11, 13, 8, 66, 0 };

        public static readonly int CryptorCodenameSize = 3;

        public static readonly int HasherCodenameSize = 3;

        public static HashAlgorithm GetHasherFromCode(string code)
        {
            if (code.Equals("md5"))
            {
                return MD5.Create();
            }
            else if (code.Equals("sh2"))
            {
                return SHA256.Create();
            }
            else
            {
                throw new UnknownHasherCodeException(code);
            }
        }

        public static string GetCodeFromHasher(HashAlgorithm hasher)
        {
            if (hasher is MD5)
            {
                return "md5";
            }
            else if (hasher is SHA256)
            {
                return "sh2";
            }
            else
            {
                throw new UnknownHasherCodeException(string.Empty);
            }
        }

        public static bool IsHasherCodeValid(string code)
        {
            return code.Equals("md5") || code.Equals("sh2");
        }
    }
}