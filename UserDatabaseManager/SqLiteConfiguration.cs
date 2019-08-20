using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace UserDatabaseManager
{
    /// <summary>
    /// Defines the <see cref="SQLiteConfiguration" /> class that is used for SQLite connection settings.
    /// </summary>
    public class SQLiteConfiguration : DbConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteConfiguration"/> class.
        /// </summary>
        public SQLiteConfiguration()
        {
            this.SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            this.SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            this.SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}