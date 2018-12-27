using Microsoft.VisualStudio.TestTools.UnitTesting;

using static BatchHost.Model.FixedValue;

namespace BatchHost.Model.Tests
{
    [TestClass()]
    public class ExtensionTests
    {
        [TestMethod()]
        public void GetMinutesTest()
        {
            Assert.AreEqual(0, Extension.GetMinutes((TimeUnits)10));
            Assert.AreEqual(1, Extension.GetMinutes(TimeUnits.Minute));
            Assert.AreEqual(60, Extension.GetMinutes(TimeUnits.Hour));
            Assert.AreEqual(1440, Extension.GetMinutes(TimeUnits.Day));
        }
    }
}