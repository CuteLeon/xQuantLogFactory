using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogFactory.Model.Fixed;
using LogFactory.Model.Result;

namespace LogFactory.BIZ.Analysiser.GroupAnalysiser.Tests
{
    [TestClass()]
    public class GroupLogAnalysiserTests
    {
        [TestMethod()]
        public void CheckMatchTest()
        {
            TerminalMonitorResult result0 = new TerminalMonitorResult()
            {
                GroupType = GroupTypes.Start,
                LogTime = DateTime.Parse("1997-1-1"),
            };
            TerminalMonitorResult result1 = new TerminalMonitorResult()
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
            Assert.IsFalse(result0.CheckMatch(result1));
        }
    }
}