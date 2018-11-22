using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class MonitorBaseTests
    {
        [TestMethod()]
        public void MonitorBaseTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "测试容器" };

            container.MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_0" });
            Assert.AreEqual(1, container.MonitorTreeRoots.Count);
            Assert.AreEqual(1, container.GetMonitorItems().Count());
            Assert.AreEqual(1, container.GetMonitorItems().Count());

            container.MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_1" });
            Assert.AreEqual(2, container.MonitorTreeRoots.Count);
            Assert.AreEqual(2, container.GetMonitorItems().Count());
            Assert.AreEqual(2, container.GetMonitorItems().Count());

            container.MonitorTreeRoots[0].MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_2" });
            Assert.AreEqual(2, container.MonitorTreeRoots.Count);
            Assert.AreEqual(3, container.GetMonitorItems().Count());
        }

    }
}