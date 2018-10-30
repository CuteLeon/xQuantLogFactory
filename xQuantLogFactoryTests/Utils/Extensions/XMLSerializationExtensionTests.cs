using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model;

namespace xQuantLogFactory.Utils.Extensions.Tests
{
    [TestClass()]
    public class XMLSerializationExtensionTests
    {
        [TestMethod()]
        public void SerializeToXMLTest()
        {
            MonitorItem rootItem = new MonitorItem
            {
                Name = "客户端启动",
                StartPattern = "客户端启动开始",
                FinishPatterny = "初始化第二阶段开始",
                ChildList = new List<MonitorItem>{
                    new MonitorItem{ Name="数据加载",
                        StartPattern="加载中债参数设置表",
                        FinishPatterny="加载当前登录部门",
                        ChildList= new List<MonitorItem>{
                            new MonitorItem{
                                Name="债券加载",
                                StartPattern="加载TBND查询",
                                FinishPatterny="加载TBND"}}}
                    }
            };

            var containor = new MonitorContainer();
            containor.ItemList = new List<MonitorItem> { rootItem, new MonitorItem { Name = "11" } };

        }

        [TestMethod()]
        public void DeserializeToObjectTest()
        {
            
        }
    }
}