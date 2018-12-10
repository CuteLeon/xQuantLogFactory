using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Utils.Extensions;

namespace xQuantLogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class MonitorContainerTests
    {
        private readonly string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MonitorRoot xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Item Name=""A"" Begin=""AS"" End=""AE"" Sheet=""A"">
    <Item Name=""A-1"" Begin=""A-1-S"" End=""A-1-E"" Sheet=""A-1"">
      <Item Name=""A-1-1"" Begin="""" End=""A-1-1-E""></Item>
      <Item Name=""A-1-2"" Begin="""" End=""A-1-2-E""></Item>
      <Item Name=""A-1-3"" Begin="""" End=""A-1-3-E""></Item>
    </Item>
    <Item Name=""A-2"" Begin="""" End=""A-2-E""></Item>
  </Item>
  <Item Name=""B"" Begin="""" End=""BE"" Sheet=""B""></Item>
  <Item Name=""C"" Begin="""" End=""CE""></Item>
  <Item Name=""D"" Begin="""" End=""DE""></Item>
  <Item Name=""E"" Begin="""" End=""EE""></Item>
</MonitorRoot>";

        [TestMethod()]
        public void InitMonitorTreeTest()
        {
            MonitorContainer container = this.xmlContent.DeserializeToObject<MonitorContainer>();

            Assert.IsNotNull(container);

            container.InitMonitorTree();

            Assert.AreEqual("A", container.MonitorTreeRoots[0].Name);
            Assert.AreEqual("A-1", container.MonitorTreeRoots[0].MonitorTreeRoots[0].Name);
            Assert.AreEqual("A-1-1", container.MonitorTreeRoots[0].MonitorTreeRoots[0].MonitorTreeRoots[0].Name);
            Assert.AreEqual("A-1-2", container.MonitorTreeRoots[0].MonitorTreeRoots[0].MonitorTreeRoots[1].Name);
            Assert.AreEqual("A-1-3", container.MonitorTreeRoots[0].MonitorTreeRoots[0].MonitorTreeRoots[2].Name);
            Assert.AreEqual("A-2", container.MonitorTreeRoots[0].MonitorTreeRoots[1].Name);
            Assert.AreEqual("B", container.MonitorTreeRoots[1].Name);
            Assert.AreEqual("C", container.MonitorTreeRoots[2].Name);
            Assert.AreEqual("D", container.MonitorTreeRoots[3].Name);
            Assert.AreEqual("E", container.MonitorTreeRoots[4].Name);

            Assert.AreEqual("A", container.MonitorTreeRoots[0].SheetName);
            Assert.AreEqual("A-1", container.MonitorTreeRoots[0].MonitorTreeRoots[0].SheetName);
            Assert.AreEqual("A-1", container.MonitorTreeRoots[0].MonitorTreeRoots[0].MonitorTreeRoots[0].SheetName);
            Assert.AreEqual("A-1", container.MonitorTreeRoots[0].MonitorTreeRoots[0].MonitorTreeRoots[1].SheetName);
            Assert.AreEqual("A-1", container.MonitorTreeRoots[0].MonitorTreeRoots[0].MonitorTreeRoots[2].SheetName);
            Assert.AreEqual("A", container.MonitorTreeRoots[0].MonitorTreeRoots[1].SheetName);
            Assert.AreEqual("B", container.MonitorTreeRoots[1].SheetName);
            Assert.AreEqual("原始", container.MonitorTreeRoots[2].SheetName);
            Assert.AreEqual("原始", container.MonitorTreeRoots[3].SheetName);
            Assert.AreEqual("原始", container.MonitorTreeRoots[4].SheetName);
        }

        [TestMethod]
        public void DeserializeToObjectTestEx()
        {
            string xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<MonitorRoot xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" Name=""交易日终清算监视容器"">
  <Item Name=""内存消耗"" Begin=""内存消耗"" End="""" Memory=""true"" Sheet=""内存""/>
  <Item Name=""日终清算"" Begin=""清算进度=0/1] 开始"" End=""清算进度=1/1] 完成------"" DirectedAnalysiser=""KeyValuePair"" Sheet=""交易清算"">
    <Item Name=""准备金融工具现金流"" End=""准备金融工具现金流""></Item>
    <Item Name=""清算前准备初始化"" Begin="""" End="" 清算前准备初始化完成""/>
    <Item Name=""清算前准备起始余额"" Begin="""" End="" 清算前准备起始余额完成""/>
    <Item Name=""清算前交易检查"" Begin="""" End=""清算前交易检查完成""/>
    <Item Name=""交易前持仓结算和托管转入"" Begin="""" End="" 清算托管转入完成""/>
    <Item Name=""清算交易"" Begin="""" End="" 清算交易(不含转托管)完成""/>
    <Item Name=""清算基金拆分和基金合并转出交易"" Begin="""" End=""清算基金拆分和基金合并转出交易完成""/>
    <Item Name=""持仓清算"" Begin="""" End="" 持仓清算完成""/>
    <Item Name=""清算基金拆分和基金合并转入交易"" Begin="""" End=""清算基金拆分转入和基金合并交易完成""/>
    <Item Name=""清算托管转出"" Begin="""" End=""清算托管转出完成""/>
    <Item Name=""移除无效指令完成"" Begin="""" End=""移除无效指令完成""/>
    <Item Name=""刷新标准券和分销额度和计算占资"" Begin="""" End=""计算占资费用完成""/>
    <Item Name=""获取清算后持仓"" Begin="""" End=""获取清算后持仓完成""/>
    <Item Name=""余额归档"" Begin="""" End=""余额归档完成""/>
    <Item Name=""清算T+1"" Begin="""" End=""清算T+1完成""/>
  </Item>
</MonitorRoot>";

            MonitorContainer container = xmlContent.DeserializeToObject<MonitorContainer>();
            container.InitMonitorTree();

            MonitorItem targetMonitor = container.MonitorTreeRoots[1];
            Assert.IsNotNull(targetMonitor);

            Assert.AreEqual(targetMonitor.StartPattern, targetMonitor.MonitorTreeRoots[0].StartPattern);
            for (int index = 1; index < targetMonitor.MonitorTreeRoots.Count; index++)
            {
                Assert.AreEqual(
                    targetMonitor.MonitorTreeRoots[index - 1].FinishPatterny,
                    targetMonitor.MonitorTreeRoots[index].StartPattern);
            }
        }
    }
}