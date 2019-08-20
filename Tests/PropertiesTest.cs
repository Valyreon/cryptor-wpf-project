using Microsoft.VisualStudio.TestTools.UnitTesting;

using PropertiesStreams;

using System.IO;

namespace Tests
{
    /// <summary>
    /// Defines the <see cref="PropertiesTest" />
    /// </summary>
    [TestClass]
    public class PropertiesTest
    {
        [TestMethod]
        public void TestPropertiesWriteAndRead()
        {
            Properties prop = new Properties();
            prop.AddProperty("file", "C:\\Path\\To\\File");
            prop.AddProperty("bool", "true");
            using (MemoryStream stream = new MemoryStream())
            {
                prop.Store(stream);

                Properties readProp = new Properties();
                readProp.Load(stream);
                Assert.IsTrue(readProp.PropertyNames.Contains("file"));
                Assert.IsTrue(readProp.PropertyNames.Contains("bool"));
                Assert.AreEqual("C:\\Path\\To\\File", readProp.GetProperty("file"));
                Assert.AreEqual("true", readProp.GetProperty("bool"));
            }
        }
    }
}