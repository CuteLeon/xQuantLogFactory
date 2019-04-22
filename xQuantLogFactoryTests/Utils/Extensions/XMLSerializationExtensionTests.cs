using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class XMLSerializationExtensionTests
    {
        [TestMethod]
        public void ReadXMLTest()
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"..\\..\\..\\xQuantLogFactory\\ReportTemplet\\SQLHash.xml");
            var sqlHashs = document.GetElementsByTagName("SQLHashs").Item(0);
            if (sqlHashs == null ||
                sqlHashs.ChildNodes.Count == 0)
            {
                Assert.Fail();
            }

            foreach (XmlNode sqlHash in sqlHashs.ChildNodes)
            {
                if ("SQLHash".Equals(sqlHash.Name, StringComparison.OrdinalIgnoreCase))
                {
                    string hash = sqlHash.Attributes["Hash"]?.Value;
                    string description = sqlHash.Attributes["Description"]?.Value;
                }
            }
        }

        [TestMethod()]
        public void SerializeToXMLTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "监听客户端启动方案", TerminalMonitorTreeRoots = new List<TerminalMonitorItem>() };

            TerminalMonitorItem clientItem = new TerminalMonitorItem
            {
                IgnoreUnIntactRoot = true,
                Name = "客户端启动",
                StartPattern = "客户端启动开始",
                FinishPattern = "初始化第二阶段开始",
                DirectedAnalysiser = TerminalDirectedAnalysiserTypes.None,
                Memory = false,
            };
            TerminalMonitorItem dataItem = new TerminalMonitorItem()
            {
                IgnoreUnIntactRoot = false,
                Name = "数据加载",
                StartPattern = "加载中债参数设置表",
                FinishPattern = "加载当前登录部门",
                DirectedAnalysiser = TerminalDirectedAnalysiserTypes.Load,
                Memory = true,
            };
            TerminalMonitorItem bondItem = new TerminalMonitorItem()
            {
                Name = "债券加载",
                StartPattern = "加载TBND查询",
                FinishPattern = "加载TBND",
                DirectedAnalysiser = TerminalDirectedAnalysiserTypes.Prefix,
            };
            TerminalMonitorItem memoryItem = new TerminalMonitorItem()
            {
                Name = "监视内存",
                StartPattern = "内存消耗",
                Memory = true,
                SheetName = "内存"
            };

            PerformanceMonitorItem logonItem = new PerformanceMonitorItem()
            {
                StartPattern = "logonstart",
                FinishPattern = "logonfinish",
                /*
                DirectedAnalysiser = DirectedAnalysiserTypes.Load,
                GroupAnalysiser = GroupAnalysiserTypes.FormAsync,
                 */
            };
            PerformanceMonitorItem logonFail = new PerformanceMonitorItem()
            {
                StartPattern = "登录失败",
                /*
                DirectedAnalysiser = DirectedAnalysiserTypes.Prefix,
                GroupAnalysiser = GroupAnalysiserTypes.SelfSealing,
                 */
            };
            PerformanceMonitorItem reportItem = new PerformanceMonitorItem()
            {
                StartPattern = "requestreport",
                /*
                SheetName = "内存",
                Memory = true,
                 */
            };

            container.TerminalMonitorTreeRoots.Add(clientItem);
            container.TerminalMonitorTreeRoots.Add(memoryItem);
            clientItem.MonitorTreeRoots.Add(dataItem);
            clientItem.MonitorTreeRoots.Add(new TerminalMonitorItem() { Name = "额外任务" });
            dataItem.MonitorTreeRoots.Add(bondItem);

            container.PerformanceMonitorTreeRoots.Add(logonItem);
            container.PerformanceMonitorTreeRoots.Add(reportItem);
            logonItem.MonitorTreeRoots.Add(logonFail);

            string xmlContent = container.SerializeToXML();
            Assert.IsNotNull(xmlContent);

            File.WriteAllText("Monitor.xml", xmlContent);
        }

        [TestMethod()]
        public void DeserializeToObjectTest()
        {
            string xmlContent = File.ReadAllText("Monitor.xml");
            if (string.IsNullOrEmpty(xmlContent))
                throw new ArgumentNullException(nameof(xmlContent));

            MonitorContainer container = xmlContent.DeserializeToObject<MonitorContainer>();

            container.InitTerminalMonitorTree();
            container.InitPerformanceMonitorTree();

            Assert.IsNotNull(container);
            Assert.AreEqual("监听客户端启动方案", container.Name);
            Assert.IsTrue(container.TerminalMonitorTreeRoots[0].IgnoreUnIntactRoot);
            Assert.IsFalse(container.TerminalMonitorTreeRoots[0].MonitorTreeRoots[0].IgnoreUnIntactRoot);
            Assert.AreEqual(2, container.TerminalMonitorTreeRoots[0].MonitorTreeRoots.Count);
            Assert.AreEqual(5, container.GetTerminalMonitorItems().Count());
            Assert.IsTrue(container.TerminalMonitorTreeRoots[1].Memory);
            Assert.AreEqual("内存", container.TerminalMonitorTreeRoots[1].SheetName);
            Assert.AreEqual(TerminalDirectedAnalysiserTypes.Load, container.TerminalMonitorTreeRoots[0].MonitorTreeRoots[0].DirectedAnalysiser);

            Assert.AreSame(null, container.TerminalMonitorTreeRoots[0].ParentMonitorItem);
            Assert.AreSame(container.TerminalMonitorTreeRoots[0], container.TerminalMonitorTreeRoots[0].MonitorTreeRoots[0].ParentMonitorItem);

            Assert.AreEqual(1, container.PerformanceMonitorTreeRoots[0].MonitorTreeRoots.Count);
            Assert.AreEqual(3, container.GetPerformanceMonitorItems().Count());
            /*
            Assert.IsTrue(container.PerformanceMonitorTreeRoots[1].Memory);
            Assert.AreEqual("内存", container.PerformanceMonitorTreeRoots[1].SheetName);
            Assert.AreEqual(DirectedAnalysiserTypes.Prefix, container.PerformanceMonitorTreeRoots[0].MonitorTreeRoots[0].DirectedAnalysiser);
             */

            Assert.AreSame(null, container.PerformanceMonitorTreeRoots[0].ParentMonitorItem);
            Assert.AreSame(container.PerformanceMonitorTreeRoots[0], container.PerformanceMonitorTreeRoots[0].MonitorTreeRoots[0].ParentMonitorItem);
        }
    }
}