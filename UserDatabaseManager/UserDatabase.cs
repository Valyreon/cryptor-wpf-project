using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UserDatabaseManager
{
    /// <summary>
    /// Defines the <see cref="UserDatabase" /> class that is used for persisting <see cref="User"/> data in a database.
    /// </summary>
    public class UserDatabase
    {
        private readonly UsersContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDatabase"/> class.
        /// </summary>
        /// <param name="pathToDatabase">The path to the database file.<see cref="string"/></param>
        public UserDatabase(string pathToDatabase)
        {
            this.context = new UsersContext(pathToDatabase);
        }

        public static byte[] Pepper { get; } = new byte[] { 43, 92, 24, 11 };

        public User GetUser(string username)
        {
            return this.context.Users.Where(u => u.Username == username).SingleOrDefault();
        }

        public User GetUserWithThumbprint(string thumbprint)
        {
            return this.context.Users.Where(u => u.CertificateThumbprint == thumbprint).SingleOrDefault();
        }

        public void AddUser(string username, string password, string thumbprint)
        {
            var hasher = SHA1.Create();
            byte[] salt = new byte[16];
            new Random().NextBytes(salt);
            byte[] passBytes = Encoding.ASCII.GetBytes(password);
            byte[] passHash = hasher.ComputeHash(Pepper.Concat(salt).Concat(passBytes).ToArray());

            User toAdd = new User
            {
                Username = username,
                CertificateThumbprint = thumbprint,
                Salt = salt,
                PassHash = passHash,
                IsExternal = false
            };

            this.context.Users.Add(toAdd);
            this.context.SaveChanges();
        }

        public void AddExternal(string username, string thumbprint)
        {
            User toAdd = new User
            {
                Username = username,
                CertificateThumbprint = thumbprint,
                IsExternal = true
            };

            this.context.Users.Add(toAdd);
            this.context.SaveChanges();
        }
    }
}