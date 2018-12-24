using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.EqualityComparer.Tests
{
    [TestClass()]
    public class LogResultEqualityComparerTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            TerminalLogFile logFile_0 = new TerminalLogFile() { FilePath = "A" };
            TerminalLogFile logFile_1 = new TerminalLogFile() { FilePath = "B" };

            TerminalMonitorResult result_0 = new TerminalMonitorResult()
            {
                LogFile = logFile_0,
                LineNumber = 0,
            };
            TerminalMonitorResult result_1 = new TerminalMonitorResult()
            {
                LogFile = logFile_1,
                LineNumber = 1,
            };
            TerminalMonitorResult result_2 = new TerminalMonitorResult()
            {
                LogFile = logFile_0,
                LineNumber = 0,
            };
            TerminalMonitorResult result_3 = new TerminalMonitorResult()
            {
                LogFile = logFile_1,
                LineNumber = 1,
            };

            Assert.IsTrue(Equals(result_0, result_2));
            Assert.IsTrue(Equals(result_1, result_3));

            Assert.IsFalse(Equals(result_0, result_1));
            Assert.IsFalse(Equals(result_1, result_2));
            Assert.IsFalse(Equals(result_2, result_3));
            Assert.IsFalse(Equals(result_3, result_0));
        }
    }
}