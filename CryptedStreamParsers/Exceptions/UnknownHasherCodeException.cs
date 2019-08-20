using System;

namespace CryptedStreamParsers.Exceptions
{
    /// <summary>
    /// Defines the <see cref="UnknownHasherCodeException" />
    /// </summary>
    public class UnknownHasherCodeException : Exception
    {
        public UnknownHasherCodeException(string code) : base("Unknown hasher with code '" + code + "'.")
        {
        }
    }
}