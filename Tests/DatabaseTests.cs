using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserDatabaseManager;

namespace Tests
{
    /// <summary>
    /// Defines the <see cref="DatabaseTests" />
    /// </summary>
    [TestClass]
    public class DatabaseTests
    {
        /*[TestMethod]
        public void TestAddToDatabase()
        {
            UserDatabase db = new UserDatabase(@"C:\Users\luka.budrak\Desktop\cryptor-wpf-project\Users.db");
            db.AddUser("default", "default", File.ReadAllBytes(@"C:\Users\luka.budrak\Desktop\cryptor-wpf-project\OPENSSL\certs\02.pem"));
            db.AddUser("luka", "luka", File.ReadAllBytes(@"C:\Users\luka.budrak\Desktop\cryptor-wpf-project\OPENSSL\certs\03.pem"));
        }*/

        [TestMethod]
        public void TestPassword()
        {
            UserDatabase db = new UserDatabase(@"F:\Documents\Visual Studio 2019\Projects\cryptor-wpf-project\Users.db");
            var st = Stopwatch.StartNew();
            db.GetUser("default");
            st.Stop();
        }
    }
}