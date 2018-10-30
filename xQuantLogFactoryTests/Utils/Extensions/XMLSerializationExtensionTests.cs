﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model;
using System.IO;

namespace xQuantLogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class XMLSerializationExtensionTests
    {

        [TestMethod()]
        public void SerializeToXMLTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "监听客户端启动方案", ItemList = new List<MonitorItem>() };

            MonitorItem rootItem = new MonitorItem
            {
                Name = "客户端启动",
                StartPattern = "客户端启动开始",
                FinishPatterny = "初始化第二阶段开始",
                ChildList = new List<MonitorItem>(),
            };
            MonitorItem dataItem = new MonitorItem()
            {
                Name = "数据加载",
                StartPattern = "加载中债参数设置表",
                FinishPatterny = "加载当前登录部门",
                ChildList = new List<MonitorItem>(),
            };
            MonitorItem bondItem = new MonitorItem()
            {
                Name = "债券加载",
                StartPattern = "加载TBND查询",
                FinishPatterny = "加载TBND",
                ChildList = new List<MonitorItem>(),
            };

            container.ItemList.Add(rootItem);
            rootItem.ChildList.Add(dataItem);
            rootItem.ChildList.Add(new MonitorItem() { Name = "额外任务" });
            dataItem.ChildList.Add(bondItem);

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
            Assert.AreEqual(2, container.ItemList[0].ChildList.Count);
        }
    }
}