using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.EqualityComparer.Tests
{
    [TestClass()]
    public class LogResultEqualityComparerTests
    {
        [TestMethod()]
        public void EqualsTest()
        {
            LogResultEqualityComparer<LogResultBase> comparer = new LogResultEqualityComparer<LogResultBase>();

            LogFile logFile_0 = new LogFile() { FilePath = "A" };
            LogFile logFile_1 = new LogFile() { FilePath = "B" };

            MonitorResult result_0 = new MonitorResult()
            {
                LogFile = logFile_0,
                LineNumber = 0,
            };
            MonitorResult result_1 = new MonitorResult()
            {
                LogFile = logFile_1,
                LineNumber = 1,
            };
            MonitorResult result_2 = new MonitorResult()
            {
                LogFile = logFile_0,
                LineNumber = 0,
            };
            MonitorResult result_3 = new MonitorResult()
            {
                LogFile = logFile_1,
                LineNumber = 1,
            };

            Assert.IsTrue(comparer.Equals(result_0, result_2));
            Assert.IsTrue(comparer.Equals(result_1, result_3));

            Assert.IsFalse(comparer.Equals(result_0, result_1));
            Assert.IsFalse(comparer.Equals(result_1, result_2));
            Assert.IsFalse(comparer.Equals(result_2, result_3));
            Assert.IsFalse(comparer.Equals(result_3, result_0));
        }
    }
}