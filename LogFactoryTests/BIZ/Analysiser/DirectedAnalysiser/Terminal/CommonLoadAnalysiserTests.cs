using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using LogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal;

namespace LogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal.Tests
{
    [TestClass()]
    public class CommonLoadAnalysiserTests
    {
        [TestMethod()]
        public void AnalysisTest()
        {
            string[] invalidLog = new string[]
            {
                "akdhhlawllh",
                "1加载TTRD_OTC_STORE：4",
                "加载：4",
                "加载TTRD_OTC_STORE4",
                "加载TTRD_OTC_STORE：哈哈哈",
                "加载TTRD_OTC_STORE：",
            };
            string[] validLog = new string[]
            {
                "加载TTRD_OTC_STORE：412",
            };

            CommonLoadAnalysiser analysiser = new CommonLoadAnalysiser();

            Assert.IsTrue(invalidLog.All(log => !analysiser.AnalysisRegex.IsMatch(log)));

            Match match = analysiser.AnalysisRegex.Match(validLog[0]);
            Assert.IsTrue(match.Success);
            Assert.IsTrue(match.Groups["ResourceName"].Success);
            Assert.IsTrue(match.Groups["Elapsed"].Success);
            Assert.AreEqual("TTRD_OTC_STORE", match.Groups["ResourceName"].Value);
            Assert.IsTrue(match.Groups["Elapsed"].Success);
            Assert.AreEqual("412", match.Groups["Elapsed"].Value);
        }
    }
}