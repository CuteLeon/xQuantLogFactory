using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils.Collections;

namespace xQuantLogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class XMLSerializationExtensionTests
    {

        [TestMethod()]
        public void SerializeToXMLTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "监听客户端启动方案", MonitorItems = new VersionedList<MonitorItem>() };

            MonitorItem rootItem = new MonitorItem
            {
                Name = "客户端启动",
                StartPattern = "客户端启动开始",
                FinishPatterny = "初始化第二阶段开始",
                MonitorItems = new VersionedList<MonitorItem>(),
            };
            MonitorItem dataItem = new MonitorItem()
            {
                Name = "数据加载",
                StartPattern = "加载中债参数设置表",
                FinishPatterny = "加载当前登录部门",
                MonitorItems = new VersionedList<MonitorItem>(),
            };
            MonitorItem bondItem = new MonitorItem()
            {
                Name = "债券加载",
                StartPattern = "加载TBND查询",
                FinishPatterny = "加载TBND",
                MonitorItems = new VersionedList<MonitorItem>(),
            };

            container.MonitorItems.Add(rootItem);
            rootItem.MonitorItems.Add(dataItem);
            rootItem.MonitorItems.Add(new MonitorItem() { Name = "额外任务" });
            dataItem.MonitorItems.Add(bondItem);

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

            Assert.IsNotNull(container);
            Assert.AreEqual("监听客户端启动方案", container.Name);
            Assert.AreEqual(2, container.MonitorItems[0].MonitorItems.Count);
        }
    }
}