using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using LogFactory.Model.Result;

namespace LogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class MonitorItemTests
    {
        [TestMethod()]
        public void MonitorItemTest()
        {
            MonitorContainer container = new MonitorContainer();
            container.TerminalMonitorTreeRoots.Add(new TerminalMonitorItem("A"));
            container.TerminalMonitorTreeRoots[0].MonitorTreeRoots.Add(new TerminalMonitorItem("A1"));
            container.TerminalMonitorTreeRoots[0].MonitorTreeRoots.Add(new TerminalMonitorItem("A2"));
            container.TerminalMonitorTreeRoots[0].MonitorTreeRoots.Add(new TerminalMonitorItem("A3"));
            container.TerminalMonitorTreeRoots.Add(new TerminalMonitorItem("B"));
            container.TerminalMonitorTreeRoots.Add(new TerminalMonitorItem("C"));
            container.TerminalMonitorTreeRoots[2].MonitorTreeRoots.Add(new TerminalMonitorItem("C1"));
            container.TerminalMonitorTreeRoots[2].MonitorTreeRoots.Add(new TerminalMonitorItem("C2"));
            container.TerminalMonitorTreeRoots.Add(new TerminalMonitorItem("D"));

            container.InitTerminalMonitorTree();
            Console.WriteLine("监视规则目录编号：");
            container.GetTerminalMonitorItems().ToList().ForEach(monitor => Console.WriteLine(monitor.CANO));

            List<TerminalMonitorResult> results = new List<TerminalMonitorResult>
            {
                new TerminalMonitorResult() { MonitorItem = container.TerminalMonitorTreeRoots[2], LogTime = DateTime.FromBinary(200) },
                new TerminalMonitorResult() { MonitorItem = container.TerminalMonitorTreeRoots[0].MonitorTreeRoots[1], LogTime = DateTime.FromBinary(100) },
                new TerminalMonitorResult() { MonitorItem = container.TerminalMonitorTreeRoots[0], LogTime = DateTime.FromBinary(100) },
                new TerminalMonitorResult() { MonitorItem = container.TerminalMonitorTreeRoots[3], LogTime = DateTime.FromBinary(300) },
                new TerminalMonitorResult() { MonitorItem = container.TerminalMonitorTreeRoots[0].MonitorTreeRoots[0], LogTime = DateTime.FromBinary(100) },
                new TerminalMonitorResult() { MonitorItem = container.TerminalMonitorTreeRoots[2].MonitorTreeRoots[0], LogTime = DateTime.FromBinary(200) }
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