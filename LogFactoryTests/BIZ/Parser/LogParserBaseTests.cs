using Microsoft.VisualStudio.TestTools.UnitTesting;

using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;

namespace LogFactory.BIZ.Parser.Tests
{
    [TestClass()]
    public class LogParserBaseTests
    {
        [TestMethod()]
        public void MatchMonitorTest()
        {
            TerminalMonitorItem monitor = new TerminalMonitorItem()
            {
                Name = "测试监视规则",
                StartPattern = "开始",
                FinishPattern = "结束",
            };

            Assert.AreEqual(GroupTypes.Unmatch, monitor.MatchGroupLog("*=*"));
            Assert.AreEqual(GroupTypes.Start, monitor.MatchGroupLog("*开始*"));
            Assert.AreEqual(GroupTypes.Finish, monitor.MatchGroupLog("*结束*"));
        }
    }
}