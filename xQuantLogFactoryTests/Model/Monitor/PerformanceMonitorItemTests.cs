using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Fixed;

namespace xQuantLogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class PerformanceMonitorItemTests
    {
        [TestMethod()]
        public void MatchGroupLogTest()
        {
            PerformanceMonitorItem monitorItem = new PerformanceMonitorItem()
            {
                StartPattern = "GetA",
                FinishPattern = "GetB",
            };

            Assert.AreEqual(GroupTypes.Unmatch, monitorItem.MatchGroupLog("GetC"));
            Assert.AreEqual(GroupTypes.Start, monitorItem.MatchGroupLog("GetA"));
            Assert.AreEqual(GroupTypes.Finish, monitorItem.MatchGroupLog("GetB"));
        }
    }
}