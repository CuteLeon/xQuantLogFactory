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
            TerminalAnalysisResultEqualityComparer<TerminalAnalysisResult> comparer = new TerminalAnalysisResultEqualityComparer<TerminalAnalysisResult>();

            TerminalLogFile logFile_0 = new TerminalLogFile() { FilePath = "A" };
            TerminalLogFile logFile_1 = new TerminalLogFile() { FilePath = "B" };

            TerminalAnalysisResult result_0 = new TerminalAnalysisResult()
            {
                LogFile = logFile_0,
                LineNumber = 0,
            };
            TerminalAnalysisResult result_1 = new TerminalAnalysisResult()
            {
                LogFile = logFile_1,
                LineNumber = 1,
            };
            TerminalAnalysisResult result_2 = new TerminalAnalysisResult()
            {
                LogFile = logFile_0,
                LineNumber = 0,
            };
            TerminalAnalysisResult result_3 = new TerminalAnalysisResult()
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