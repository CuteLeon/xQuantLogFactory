using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model.Fixed;
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
                GroupType = GroupTypes.Start,
                LogTime = DateTime.Parse("1997-1-1"),
            };
            MonitorResult result1 = new MonitorResult()
            {
                GroupType = GroupTypes.Finish,
                LogTime = DateTime.Parse("1997-1-1"),
            };

            result0.Version = "versionA";
            Assert.IsTrue(result0.CheckMatch(result1));
            result1.Version = "versionB";
            Assert.IsFalse(result0.CheckMatch(result1));
            result1.Version = "versionA";
            Assert.IsTrue(result0.CheckMatch(result1));
            result0.Version = "";
            Assert.IsTrue(result0.CheckMatch(result1));

            result0.Client = "clientA";
            Assert.IsTrue(result0.CheckMatch(result1));
            result1.Client = "clientB";
            Assert.IsFalse(result0.CheckMatch(result1));
            result1.Client = "clientA";
            Assert.IsTrue(result0.CheckMatch(result1));
            result0.Client = "";
            Assert.IsTrue(result0.CheckMatch(result1));

            result0.LogTime = result0.LogTime.AddDays(1);
            Assert.IsFalse(analysiser.CheckMatch(result0, result1));
        }
    }
}