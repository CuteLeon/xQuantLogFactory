using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class MonitorItemTests
    {
        [TestMethod()]
        public void MonitorItemTest()
        {
            MonitorContainer container = new MonitorContainer();
            container.MonitorTreeRoots.Add(new MonitorItem("A"));
            container.MonitorTreeRoots[0].MonitorTreeRoots.Add(new MonitorItem("A1"));
            container.MonitorTreeRoots[0].MonitorTreeRoots.Add(new MonitorItem("A2"));
            container.MonitorTreeRoots[0].MonitorTreeRoots.Add(new MonitorItem("A3"));
            container.MonitorTreeRoots.Add(new MonitorItem("B"));
            container.MonitorTreeRoots.Add(new MonitorItem("C"));
            container.MonitorTreeRoots[2].MonitorTreeRoots.Add(new MonitorItem("C1"));
            container.MonitorTreeRoots[2].MonitorTreeRoots.Add(new MonitorItem("C2"));
            container.MonitorTreeRoots.Add(new MonitorItem("D"));

            container.InitMonitorTree();
            Console.WriteLine("监视规则目录编号：");
            container.GetMonitorItems().ToList().ForEach(monitor => Console.WriteLine(monitor.CANO));

            List<MonitorResult> results = new List<MonitorResult>
            {
                new MonitorResult() { MonitorItem = container.MonitorTreeRoots[2], LogTime = DateTime.FromBinary(200) },
                new MonitorResult() { MonitorItem = container.MonitorTreeRoots[0].MonitorTreeRoots[1], LogTime = DateTime.FromBinary(100) },
                new MonitorResult() { MonitorItem = container.MonitorTreeRoots[0], LogTime = DateTime.FromBinary(100) },
                new MonitorResult() { MonitorItem = container.MonitorTreeRoots[3], LogTime = DateTime.FromBinary(300) },
                new MonitorResult() { MonitorItem = container.MonitorTreeRoots[0].MonitorTreeRoots[0], LogTime = DateTime.FromBinary(100) },
                new MonitorResult() { MonitorItem = container.MonitorTreeRoots[2].MonitorTreeRoots[0], LogTime = DateTime.FromBinary(200) }
            };

            Console.WriteLine("监视结果关联监视规则目录编号：");
            results.ForEach(result => Console.WriteLine(result.MonitorItem.CANO));

            Assert.AreEqual("0003", results[0].MonitorItem.CANO);
            Assert.AreEqual("0001.0002", results[1].MonitorItem.CANO);
            Assert.AreEqual("0001", results[2].MonitorItem.CANO);
            Assert.AreEqual("0004", results[3].MonitorItem.CANO);
            Assert.AreEqual("0001.0001", results[4].MonitorItem.CANO);
            Assert.AreEqual("0003.0001", results[5].MonitorItem.CANO);

            Console.WriteLine("监视结果以日志时间排序：");
            results = results.OrderBy(result => result.LogTime).ToList();
            results.ForEach(result => Console.WriteLine(result.MonitorItem.CANO));

            Assert.AreEqual("0001.0002", results[0].MonitorItem.CANO);
            Assert.AreEqual("0001", results[1].MonitorItem.CANO);
            Assert.AreEqual("0001.0001", results[2].MonitorItem.CANO);
            Assert.AreEqual("0003", results[3].MonitorItem.CANO);
            Assert.AreEqual("0003.0001", results[4].MonitorItem.CANO);
            Assert.AreEqual("0004", results[5].MonitorItem.CANO);

            Console.WriteLine("监视结果以日志时间和监视规则目录编号排序：");
            results = results.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();
            results.ForEach(result => Console.WriteLine(result.MonitorItem.CANO));

            Assert.AreEqual("0001", results[0].MonitorItem.CANO);
            Assert.AreEqual("0001.0001", results[1].MonitorItem.CANO);
            Assert.AreEqual("0001.0002", results[2].MonitorItem.CANO);
            Assert.AreEqual("0003", results[3].MonitorItem.CANO);
            Assert.AreEqual("0003.0001", results[4].MonitorItem.CANO);
            Assert.AreEqual("0004", results[5].MonitorItem.CANO);
        }
    }
}