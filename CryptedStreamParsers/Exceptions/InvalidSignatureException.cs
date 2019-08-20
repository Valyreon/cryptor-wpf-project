using System;

namespace CryptedStreamParsers.Exceptions
{
    /// <summary>
    /// Defines the <see cref="InvalidSignatureException" />
    /// </summary>
    public class InvalidSignatureException : Exception
    {
        public InvalidSignatureException() : base("DSA Signature of the file is invalid.")
        {
        }
    }
}