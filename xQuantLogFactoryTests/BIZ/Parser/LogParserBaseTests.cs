using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.BIZ.Parser.Tests
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

            Assert.AreEqual(GroupTypes.Unmatch, monitor.MatchLog("*=*"));
            Assert.AreEqual(GroupTypes.Start, monitor.MatchLog("*开始*"));
            Assert.AreEqual(GroupTypes.Finish, monitor.MatchLog("*结束*"));
        }
    }
}