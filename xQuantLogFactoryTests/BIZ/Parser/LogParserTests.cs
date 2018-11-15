using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.BIZ.Parser.Tests
{
    [TestClass()]
    public class LogParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            string generalLog = "2018-10-29 16:35:32,651 TRACE 客户端初始化应用程序相关属性";
            string ServerLog = "2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券";
            string ClientLog = "2018-10-29 16:51:04,457 TRACE 安信证券 1.3.0.065 192.168.7.101 初始化准备";
            string MiddlewareLog = "2018-11-01 10:41:39.968	172.28.40.110	leon	2018-11-01 10:41:39.968	10	/LogonManager.soap	GetMessageList	100	开始";

            LogParserBase serverParser = new ServerLogParser();
            LogParserBase clientParser = new ClientLogParser();
            LogParserBase middlewareParser = new MiddlewareLogParser();

            Match generalMatch = middlewareParser.GeneralLogRegex.Match(generalLog);

            Assert.AreEqual("2018-10-29 16:35:32", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("651", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("客户端初始化应用程序相关属性", generalMatch.Groups["LogContent"].Value);

            generalMatch = serverParser.GeneralLogRegex.Match(ServerLog);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-10-30 09:25:30", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("111", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("东方证券 1.3.0.064补丁1 开始排券", generalMatch.Groups["LogContent"].Value);
            Match serverMatch = serverParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            Assert.AreEqual("东方证券", serverMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.064补丁1", serverMatch.Groups["Version"].Value);
            Assert.AreEqual("", serverMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("开始排券", serverMatch.Groups["LogContent"].Value);

            generalMatch = clientParser.GeneralLogRegex.Match(ClientLog);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-10-29 16:51:04", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("457", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("安信证券 1.3.0.065 192.168.7.101 初始化准备", generalMatch.Groups["LogContent"].Value);
            Match clientMatch = clientParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            Assert.AreEqual("安信证券", clientMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.065", clientMatch.Groups["Version"].Value);
            Assert.AreEqual("192.168.7.101", clientMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("初始化准备", clientMatch.Groups["LogContent"].Value);

            Match middlewareMatch = middlewareParser.ParticularRegex.Match(MiddlewareLog);
            Assert.AreEqual("2018-11-01 10:41:39.968", middlewareMatch.Groups["LogTime"].Value);
            Assert.AreEqual("172.28.40.110", middlewareMatch.Groups["Client"].Value);
            Assert.AreEqual("leon", middlewareMatch.Groups["UserCode"].Value);
            Assert.AreEqual("2018-11-01 10:41:39.968", middlewareMatch.Groups["StartTime"].Value);
            Assert.AreEqual("10", middlewareMatch.Groups["Elapsed"].Value);
            Assert.AreEqual("/LogonManager.soap", middlewareMatch.Groups["RequestURI"].Value);
            Assert.AreEqual("GetMessageList", middlewareMatch.Groups["MethodName"].Value);
            Assert.AreEqual("100", middlewareMatch.Groups["StreamLength"].Value);
            Assert.AreEqual("开始", middlewareMatch.Groups["Message"].Value);
        }
    }
}