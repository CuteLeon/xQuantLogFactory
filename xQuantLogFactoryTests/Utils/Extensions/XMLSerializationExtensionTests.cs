using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class XMLSerializationExtensionTests
    {

        [TestMethod()]
        public void SerializeToXMLTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "监听客户端启动方案", MonitorTreeRoots = new List<TerminalMonitorItem>() };

            TerminalMonitorItem clientItem = new TerminalMonitorItem
            {
                Name = "客户端启动",
                StartPattern = "客户端启动开始",
                FinishPatterny = "初始化第二阶段开始",
                DirectedAnalysiser = DirectedAnalysiserTypes.None,
                Memory = false,
            };
            TerminalMonitorItem dataItem = new TerminalMonitorItem()
            {
                Name = "数据加载",
                StartPattern = "加载中债参数设置表",
                FinishPatterny = "加载当前登录部门",
                DirectedAnalysiser = DirectedAnalysiserTypes.Load,
                Memory = true,
            };
            TerminalMonitorItem bondItem = new TerminalMonitorItem()
            {
                Name = "债券加载",
                StartPattern = "加载TBND查询",
                FinishPatterny = "加载TBND",
                DirectedAnalysiser = DirectedAnalysiserTypes.Prefix,
            };
            TerminalMonitorItem memoryItem = new TerminalMonitorItem()
            {
                Name = "监视内存",
                StartPattern = "内存消耗",
                Memory = true,
                SheetName = "内存"
            };

            container.MonitorTreeRoots.Add(clientItem);
            container.MonitorTreeRoots.Add(memoryItem);
            clientItem.MonitorTreeRoots.Add(dataItem);
            clientItem.MonitorTreeRoots.Add(new TerminalMonitorItem() { Name = "额外任务" });
            dataItem.MonitorTreeRoots.Add(bondItem);

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

            Assert.IsNotNull(container);
            Assert.AreEqual("监听客户端启动方案", container.Name);
            Assert.AreEqual(2, container.MonitorTreeRoots[0].MonitorTreeRoots.Count);
            Assert.AreEqual(5, container.GetTerminalMonitorItems().Count());
            Assert.IsTrue(container.MonitorTreeRoots[1].Memory);
            Assert.AreEqual("内存", container.MonitorTreeRoots[1].SheetName);
            Assert.AreEqual(DirectedAnalysiserTypes.Load, container.MonitorTreeRoots[0].MonitorTreeRoots[0].DirectedAnalysiser);

            Assert.AreSame(null, container.MonitorTreeRoots[0].ParentMonitorItem);
            Assert.AreSame(container.MonitorTreeRoots[0], container.MonitorTreeRoots[0].MonitorTreeRoots[0].ParentMonitorItem);
        }
    }
}