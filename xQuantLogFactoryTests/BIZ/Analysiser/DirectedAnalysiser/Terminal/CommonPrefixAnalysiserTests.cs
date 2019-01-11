using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal.Tests
{
    [TestClass()]
    public class CommonPrefixAnalysiserTests
    {
        [TestMethod()]
        public void TryGetOrAddChildMonitorTest()
        {
            TerminalMonitorItem rootItem = new TerminalMonitorItem() { CANO = "0005" };
            Assert.AreEqual("0005", rootItem.CANO);

            new TerminalMonitorItem("已存在的规则").BindParentMonitor(rootItem, true);
            Assert.AreEqual("0005.0001", rootItem.MonitorTreeRoots[0].CANO);

            CommonPrefixAnalysiser analysiser = new CommonPrefixAnalysiser();

            TerminalMonitorItem item_0 = analysiser.TryGetOrAddChildMonitor(rootItem, "已存在的规则");
            Assert.AreEqual("已存在的规则", item_0.Name);
            Assert.AreEqual(rootItem, item_0.ParentMonitorItem);
            Assert.AreEqual(1, rootItem.MonitorTreeRoots.Count);

            TerminalMonitorItem item_1 = analysiser.TryGetOrAddChildMonitor(rootItem, "不存在的规则");
            Assert.AreEqual("不存在的规则", item_1.Name);
            Assert.AreEqual("0005.0002", item_1.CANO);
            Assert.AreEqual(rootItem, item_1.ParentMonitorItem);
            Assert.AreEqual(2, rootItem.MonitorTreeRoots.Count);
        }
    }
}