using System.Data.Entity;
using System.Data.SQLite;

namespace UserDatabaseManager
{
    /// <summary>
    /// Defines the <see cref="UsersContext" /> class for Entity Framework.
    /// </summary>
    internal class UsersContext : DbContext
    {
        public UsersContext(string source) :
            base(
                new SQLiteConnection()
                {
                    ConnectionString = new SQLiteConnectionStringBuilder()
                    {
                        DataSource = source
                    }
                    .ConnectionString
                },
                true)
        {
            DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        }

        public DbSet<User> Users { get; set; }
    }
}