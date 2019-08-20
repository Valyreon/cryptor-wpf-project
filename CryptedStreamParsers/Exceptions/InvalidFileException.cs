using System;

namespace CryptedStreamParsers.Exceptions
{
    /// <summary>
    /// Defines the <see cref="InvalidFileException" />
    /// </summary>
    public class InvalidFileException : Exception
    {
        public InvalidFileException() : base("This file does not have required format.")
        {
        }
    }
}