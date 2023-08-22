using System;
using System.Collections.Generic;
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
            context = new UsersContext(pathToDatabase);
        }

        public static byte[] Pepper { get; } = new byte[] { 43, 92, 24, 11 };

        public User GetUser(string username)
        {
            return context.Users.Where(u => u.Username == username).SingleOrDefault();
        }

        public void AddUser(string username, string password, byte[] certificate)
        {
            using var hasher = SHA1.Create();
            var salt = new byte[16];
            new Random().NextBytes(salt);
            var passBytes = Encoding.ASCII.GetBytes(password);
            var passHash = hasher.ComputeHash(Pepper.Concat(salt).Concat(passBytes).ToArray());

            var toAdd = new User
            {
                Username = username,
                PublicCertificate = certificate,
                Salt = salt,
                PassHash = passHash,
                IsExternal = false
            };

            context.Users.Add(toAdd);
            context.SaveChanges();
        }

        public void AddExternal(string username, byte[] certificate)
        {
            var toAdd = new User
            {
                Username = username,
                IsExternal = true,
                PublicCertificate = certificate
            };

            context.Users.Add(toAdd);
            context.SaveChanges();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return context.Users.AsEnumerable();
        }
    }
}
