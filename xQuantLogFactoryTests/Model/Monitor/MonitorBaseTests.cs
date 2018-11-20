using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Assert.AreEqual(1, container.MonitorItems.Count);
            Assert.AreEqual(1, container.MonitorItems.Count);
            
            container.MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_1" });
            Assert.AreEqual(2, container.MonitorTreeRoots.Count);
            Assert.AreEqual(2, container.MonitorItems.Count);
            Assert.AreEqual(2, container.MonitorItems.Count);

            container.MonitorTreeRoots[0].MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_2" });
            Assert.AreEqual(2, container.MonitorTreeRoots.Count);

            //TODO: 子节点列表更新时，依然无法触发父节点更新，思路需要改一下（效率+空间复杂度）
            Assert.AreEqual(3, container.MonitorItems.Count);
        }
    }
}