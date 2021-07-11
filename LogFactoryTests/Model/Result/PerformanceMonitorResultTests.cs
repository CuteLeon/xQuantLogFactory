using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogFactory.Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogFactory.Model.Fixed;

namespace LogFactory.Model.Result.Tests
{
    [TestClass()]
    public class PerformanceMonitorResultTests
    {
        [TestMethod()]
        public void CheckMatchTest()
        {
            PerformanceMonitorResult result0 = new PerformanceMonitorResult()
            {
                GroupType = GroupTypes.Start,
                LogTime = DateTime.Parse("1997-1-1"),
            };
            PerformanceMonitorResult result1 = new PerformanceMonitorResult()
            {
                GroupType = GroupTypes.Finish,
                LogTime = DateTime.Parse("1997-1-1"),
            };

            result0.IPAddress = "10.10.10.10";
            Assert.IsTrue(result0.CheckMatch(result1));
            result1.IPAddress = "88.88.88.88";
            Assert.IsFalse(result0.CheckMatch(result1));
            result1.IPAddress = "10.10.10.10";
            Assert.IsTrue(result0.CheckMatch(result1));
            result0.IPAddress = "";
            Assert.IsTrue(result0.CheckMatch(result1));

            result0.UserCode = "leon";
            Assert.IsTrue(result0.CheckMatch(result1));
            result1.UserCode = "lzc";
            Assert.IsFalse(result0.CheckMatch(result1));
            result1.UserCode = "leon";
            Assert.IsTrue(result0.CheckMatch(result1));
            result0.UserCode = "";
            Assert.IsTrue(result0.CheckMatch(result1));

            result0.LogTime = result0.LogTime.AddDays(1);
            Assert.IsFalse(result0.CheckMatch(result1));
        }
    }
}