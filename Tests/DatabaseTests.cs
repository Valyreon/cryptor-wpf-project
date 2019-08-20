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
            UserDatabase db = new UserDatabase("C:\\Users\\luka.budrak\\Downloads\\SQLiteStudio-3.2.1\\Users.db");

            string path = "C:\\Users\\luka.budrak\\Downloads\\certificateExamples";
            CertificateManagerSettings settings = new CertificateManagerSettings(null, path);
            CertificateManager manager = new CertificateManager(settings);

            db.AddUser("test", "default", manager.Certificates[0].Thumbprint);
        }*/

        [TestMethod]
        public void TestPassword()
        {
            UserDatabase db = new UserDatabase("C:\\Users\\luka.budrak\\Downloads\\SQLiteStudio-3.2.1\\Users.db");
            User def = db.GetUser("default");
            Assert.IsTrue(def.IsPasswordValid("default"));
        }
    }
}