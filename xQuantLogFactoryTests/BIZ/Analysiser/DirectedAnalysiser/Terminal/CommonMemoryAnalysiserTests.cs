using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal.Tests
{
    [TestClass()]
    public class CommonMemoryAnalysiserTests
    {
        [TestMethod()]
        public void CommonMemoryAnalysiserTest()
        {
            string log_0 = "内存消耗：PagedMem=157.8203125、VirtualMem=5180.30078125、RamMem=203.6015625、VirtualMemAdd=0.0625";
            string log_1 = "内存消耗：PagedMem=1015.95703125、VirtualMem=6052.078125、RamMem=1050.51953125、VirtualMemAdd=202.4609375；CPU Usage=120.34";

            CommonMemoryAnalysiser analysiser = new CommonMemoryAnalysiser();

            Match match_0 = analysiser.AnalysisRegex.Match(log_0);
            Assert.AreEqual("5180.30078125", match_0.Groups["Memory"].Value);
            Assert.AreEqual(string.Empty, match_0.Groups["CPU"].Value);

            Match match_1 = analysiser.AnalysisRegex.Match(log_1);
            Assert.AreEqual("6052.078125", match_1.Groups["Memory"].Value);
            Assert.AreEqual("120.34", match_1.Groups["CPU"].Value);
        }
    }
}