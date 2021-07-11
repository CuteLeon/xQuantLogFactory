using Microsoft.VisualStudio.TestTools.UnitTesting;

using LogFactory.Model.Fixed;

namespace LogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class EnumExtensionTests
    {
        [TestMethod()]
        public void GetAmbientValueTest()
        {
            Assert.AreEqual("xlsx", ReportModes.Excel.GetAmbientValue());
            Assert.AreEqual("html", ReportModes.HTML.GetAmbientValue());
            Assert.AreEqual("doc", ReportModes.Word.GetAmbientValue());
        }
    }
}