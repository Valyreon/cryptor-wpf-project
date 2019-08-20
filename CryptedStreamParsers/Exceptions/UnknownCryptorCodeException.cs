using System;

namespace CryptedStreamParsers.Exceptions
{
    /// <summary>
    /// Defines the <see cref="UnknownCryptorCodeException" />
    /// </summary>
    public class UnknownCryptorCodeException : Exception
    {
        public UnknownCryptorCodeException(string code) : base("Unknown hasher with code '" + code + "'.")
        {
        }
    }
}