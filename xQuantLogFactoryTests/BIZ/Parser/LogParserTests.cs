using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Parser.Tests
{
    [TestClass()]
    public class LogParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            string IgoreLog = "2018-10-29 16:35:32,651 TRACE 客户端初始化应用程序相关属性";
            string ServerLog = "2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券";
            string ClientLog = "2018-10-29 16:51:04,457 TRACE 安信证券 1.3.0.065 192.168.7.101 初始化准备";

            LogParserBase serverParser = new ServerLogParser();
            Assert.IsFalse(serverParser.LogRegex.IsMatch(IgoreLog));
            Assert.IsTrue(serverParser.LogRegex.IsMatch(ClientLog));

            LogParserBase clientParser = new ClientLogParser();
            Assert.IsFalse(clientParser.LogRegex.IsMatch(IgoreLog));
            Assert.IsFalse(clientParser.LogRegex.IsMatch(ServerLog));

            Match serverMatch = serverParser.LogRegex.Match(ServerLog);

            Assert.IsTrue(serverMatch.Success);
            Assert.AreEqual("2018-10-30 09:25:30", serverMatch.Groups["LogTime"].Value);
            Assert.AreEqual("111", serverMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", serverMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("东方证券", serverMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.064补丁1", serverMatch.Groups["Version"].Value);
            Assert.AreEqual("", serverMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("开始排券", serverMatch.Groups["LogContent"].Value);

            Match clientMatch = clientParser.LogRegex.Match(ClientLog);

            Assert.IsTrue(clientMatch.Success);
            Assert.AreEqual("2018-10-29 16:51:04", clientMatch.Groups["LogTime"].Value);
            Assert.AreEqual("457", clientMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", clientMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("安信证券", clientMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.065", clientMatch.Groups["Version"].Value);
            Assert.AreEqual("192.168.7.101", clientMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("初始化准备", clientMatch.Groups["LogContent"].Value);
        }
    }
}