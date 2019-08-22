using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UserDatabaseManager
{
    /// <summary>
    /// Defines the <see cref="User" /> class that is used for storing information about a user.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] Salt { get; set; }

        public byte[] PassHash { get; set; }

        public bool IsExternal { get; set; }

        public byte[] PublicCertificate { get; set; }

        public bool IsPasswordValid(string password)
        {
            using (var hasher = SHA1.Create())
            {
                byte[] passBytes = Encoding.ASCII.GetBytes(password);
                var currentHash = hasher.ComputeHash(UserDatabase.Pepper.Concat(this.Salt).Concat(passBytes).ToArray());

                return currentHash.SequenceEqual(this.PassHash);
            }
        }
    }
}