using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.BIZ.Analysiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.BIZ.Analysiser.Tests
{
    [TestClass()]
    public class GroupLogAnalysiserTests
    {
        [TestMethod()]
        public void CheckMatchTest()
        {
            GroupLogAnalysiser analysiser = new GroupLogAnalysiser();

            MonitorResult result0 = new MonitorResult()
            {
                LogTime = DateTime.Parse("1997-1-1"),
            };
            MonitorResult result1 = new MonitorResult()
            {
                LogTime = DateTime.Parse("1997-1-1"),
            };

            result0.Version = "versionA";
            Assert.IsTrue(analysiser.CheckMatch(result0, result1));
            result1.Version = "versionB";
            Assert.IsFalse(analysiser.CheckMatch(result0, result1));
            result1.Version = "versionA";
            Assert.IsTrue(analysiser.CheckMatch(result0, result1));
            result0.Version = "";
            Assert.IsTrue(analysiser.CheckMatch(result0, result1));

            result0.Client = "clientA";
            Assert.IsTrue(analysiser.CheckMatch(result0, result1));
            result1.Client = "clientB";
            Assert.IsFalse(analysiser.CheckMatch(result0, result1));
            result1.Client = "clientA";
            Assert.IsTrue(analysiser.CheckMatch(result0, result1));
            result0.Client = "";
            Assert.IsTrue(analysiser.CheckMatch(result0, result1));

            result0.LogTime = result0.LogTime.AddDays(1);
            Assert.IsFalse(analysiser.CheckMatch(result0, result1));
        }
    }
}