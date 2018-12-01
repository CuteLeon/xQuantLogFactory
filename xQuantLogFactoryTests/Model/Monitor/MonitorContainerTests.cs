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
    }
}