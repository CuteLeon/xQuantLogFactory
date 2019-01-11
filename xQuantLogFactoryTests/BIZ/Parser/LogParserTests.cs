using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.BIZ.Parser.Tests
{
    [TestClass()]
    public class LogParserTests
    {
        [TestMethod()]
        public void ClientTerminalParseTest()
        {
            string log = "2018-10-29 16:51:04,457 TRACE 安信证券 1.3.0.065 192.168.7.101 初始化准备";

            TerminalLogParserBase parser = new ClientTerminalParser();
            Match match = parser.GeneralLogRegex.Match(log);

            Assert.IsTrue(match.Success);
            Assert.IsTrue(match.Groups["LogContent"].Success);
            Assert.AreEqual("2018-10-29 16:51:04", match.Groups["LogTime"].Value);
            Assert.AreEqual("457", match.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", match.Groups["LogLevel"].Value);
            Assert.AreEqual("安信证券 1.3.0.065 192.168.7.101 初始化准备", match.Groups["LogContent"].Value);
            Match clientMatch = parser.ParticularRegex.Match(match.Groups["LogContent"].Value);
            Assert.IsTrue(clientMatch.Success);
            Assert.AreEqual("安信证券", clientMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.065", clientMatch.Groups["Version"].Value);
            Assert.AreEqual("192.168.7.101", clientMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("初始化准备", clientMatch.Groups["LogContent"].Value);
        }

        [TestMethod()]
        public void ServerTerminalParseTest()
        {
            string log_0 = "2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券";
            string log_1 = "2018-11-23 09:00:46,534 DEBUG 宏源证券 063补丁4_宏源 日终清算[清算日期=2018-11-22, 清算进度=1/13,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 开始------";
            string log_2 = "2018-11-23 09:00:46,597 DEBUG 宏源证券 063补丁4_宏源 ....日终清算[清算日期=2018-11-22,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 清算前准备初始化完成;";
            string log_3 = "2018-12-11 20:28:12,140 TRACE 中间件进程启动开始, 进程号[13800], 中间件启动";

            TerminalLogParserBase serverParser = new ServerTerminalParser();
            Match generalMatch = serverParser.GeneralLogRegex.Match(log_0);

            Assert.IsTrue(generalMatch.Success);
            Assert.AreEqual("2018-10-30 09:25:30", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("111", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("东方证券 1.3.0.064补丁1 开始排券", generalMatch.Groups["LogContent"].Value);

            generalMatch = serverParser.GeneralLogRegex.Match(log_0);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-10-30 09:25:30", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("111", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("东方证券 1.3.0.064补丁1 开始排券", generalMatch.Groups["LogContent"].Value);
            Match serverMatch = serverParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            Assert.IsTrue(serverMatch.Success);
            Assert.AreEqual("东方证券", serverMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.064补丁1", serverMatch.Groups["Version"].Value);
            Assert.AreEqual("", serverMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("开始排券", serverMatch.Groups["LogContent"].Value);

            generalMatch = serverParser.GeneralLogRegex.Match(log_1);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-11-23 09:00:46", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("534", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("宏源证券 063补丁4_宏源 日终清算[清算日期=2018-11-22, 清算进度=1/13,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 开始------", generalMatch.Groups["LogContent"].Value);
            serverMatch = serverParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            Assert.IsTrue(serverMatch.Success);
            Assert.AreEqual("宏源证券", serverMatch.Groups["Client"].Value);
            Assert.AreEqual("063补丁4_宏源", serverMatch.Groups["Version"].Value);
            Assert.AreEqual("", serverMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("日终清算[清算日期=2018-11-22, 清算进度=1/13,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 开始------", serverMatch.Groups["LogContent"].Value);

            generalMatch = serverParser.GeneralLogRegex.Match(log_2);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-11-23 09:00:46", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("597", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("宏源证券 063补丁4_宏源 ....日终清算[清算日期=2018-11-22,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 清算前准备初始化完成;", generalMatch.Groups["LogContent"].Value);
            serverMatch = serverParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            Assert.IsTrue(serverMatch.Success);
            Assert.AreEqual("宏源证券", serverMatch.Groups["Client"].Value);
            Assert.AreEqual("063补丁4_宏源", serverMatch.Groups["Version"].Value);
            Assert.AreEqual("", serverMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("....日终清算[清算日期=2018-11-22,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 清算前准备初始化完成;", serverMatch.Groups["LogContent"].Value);

            generalMatch = serverParser.GeneralLogRegex.Match(log_3);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-12-11 20:28:12", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("140", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("中间件进程启动开始, 进程号[13800], 中间件启动", generalMatch.Groups["LogContent"].Value);
            serverMatch = serverParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            // 防止因为日志内容存在空格，而被正则错误分隔
            Assert.IsFalse(serverMatch.Success);
        }

        [TestMethod()]
        public void ClientPerformanceParseTest()
        {
            string log = "2019-01-09 10:30:03.350	2019-01-09 10:30:03.171	2019-01-09 10:30:03.185	2019-01-09 10:30:03.348	2019-01-09 10:30:03.350	178	tcp://127.0.0.1:5555/LogonManager.soap	GetLogonType	123	234";

            PerformanceLogParserBase parser = new ClientPerformanceParser();
            Match match = parser.GeneralLogRegex.Match(log);

            Assert.IsTrue(match.Success);
            Assert.AreEqual("2019-01-09 10:30:03.350", match.Groups["LogTime"].Value);
            Assert.AreEqual("2019-01-09 10:30:03.171", match.Groups["RequestSendTime"].Value);
            Assert.AreEqual("2019-01-09 10:30:03.185", match.Groups["RequestReceiveTime"].Value);
            Assert.AreEqual("2019-01-09 10:30:03.348", match.Groups["ResponseSendTime"].Value);
            Assert.AreEqual("2019-01-09 10:30:03.350", match.Groups["ResponseReceiveTime"].Value);
            Assert.AreEqual("178", match.Groups["Elapsed"].Value);
            Assert.AreEqual("123", match.Groups["RequestStreamLength"].Value);
            Assert.AreEqual("234", match.Groups["ResponseStreamLength"].Value);
            Assert.AreEqual("tcp://127.0.0.1:5555/LogonManager.soap", match.Groups["RequestURI"].Value);
            Assert.AreEqual("GetLogonType", match.Groups["MethodName"].Value);
        }

        [TestMethod()]
        public void ServerPerformanceParseTest()
        {
            string log = "2019-01-09 10:30:03.349	127.0.0.1		2019-01-09 10:30:03.185	2019-01-09 10:30:03.348	162	/LogonManager.soap	GetLogonType	0	552	完成";

            PerformanceLogParserBase parser = new ServerPerformanceParser();
            Match match = parser.GeneralLogRegex.Match(log);

            Assert.IsTrue(match.Success);
            Assert.AreEqual("2019-01-09 10:30:03.349", match.Groups["LogTime"].Value);
            Assert.AreEqual("127.0.0.1", match.Groups["IPAddress"].Value);
            Assert.AreEqual("", match.Groups["UserCode"].Value);
            Assert.AreEqual("2019-01-09 10:30:03.185", match.Groups["RequestReceiveTime"].Value);
            Assert.AreEqual("2019-01-09 10:30:03.348", match.Groups["ResponseSendTime"].Value);
            Assert.AreEqual("162", match.Groups["Elapsed"].Value);
            Assert.AreEqual("/LogonManager.soap", match.Groups["RequestURI"].Value);
            Assert.AreEqual("GetLogonType", match.Groups["MethodName"].Value);
            Assert.AreEqual("0", match.Groups["RequestStreamLength"].Value);
            Assert.AreEqual("552", match.Groups["ResponseStreamLength"].Value);
            Assert.AreEqual("完成", match.Groups["Message"].Value);
        }
    }
}