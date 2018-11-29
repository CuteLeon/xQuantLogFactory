using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.BIZ.Analysiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.BIZ.Analysiser.Tests
{
    [TestClass()]
    public class CommonPrefixAnalysiserTests
    {
        [TestMethod()]
        public void TryGetOrAddChildMonitorTest()
        {
            MonitorItem rootItem = new MonitorItem();
            rootItem.MonitorTreeRoots.Add(new MonitorItem("已存在的规则"));
            rootItem.MonitorTreeRoots[0].BindParentMonitor(rootItem);

            CommonPrefixAnalysiser analysiser = new CommonPrefixAnalysiser();

            MonitorItem item_0 = analysiser.TryGetOrAddChildMonitor(rootItem, "已存在的规则");
            Assert.AreEqual("已存在的规则", item_0.Name);
            Assert.AreEqual(rootItem, item_0.ParentMonitorItem);
            Assert.AreEqual(1, rootItem.MonitorTreeRoots.Count);

            MonitorItem item_1 = analysiser.TryGetOrAddChildMonitor(rootItem, "不存在的规则");
            Assert.AreEqual("不存在的规则", item_1.Name);
            Assert.AreEqual(rootItem, item_1.ParentMonitorItem);
            Assert.AreEqual(2, rootItem.MonitorTreeRoots.Count);
        }
    }
}