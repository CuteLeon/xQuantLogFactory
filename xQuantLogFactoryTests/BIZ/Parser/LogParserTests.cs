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
            string ServerLog_0 = "2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券";
            string ServerLog_1 = "2018-11-23 09:00:46,534 DEBUG 宏源证券 063补丁4_宏源 日终清算[清算日期=2018-11-22, 清算进度=1/13,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 开始------";
            string ServerLog_2 = "2018-11-23 09:00:46,597 DEBUG 宏源证券 063补丁4_宏源 ....日终清算[清算日期=2018-11-22,当前账户=CASH_GSLC_CJHX_TL21H,外部账户=GSLC_JJ_CASH_EXT] 清算前准备初始化完成;";
            string ServerLog_3 = "2018-12-11 20:28:12,140 TRACE 中间件进程启动开始, 进程号[13800], 中间件启动";
            string ClientLog = "2018-10-29 16:51:04,457 TRACE 安信证券 1.3.0.065 192.168.7.101 初始化准备";
            string MiddlewareLog = "2018-11-01 10:41:39.968	172.28.40.110	leon	2018-11-01 10:41:39.968	10	/LogonManager.soap	GetMessageList	100	开始";

            TerminalLogParserBase serverParser = new ServerLogParser();
            TerminalLogParserBase clientParser = new ClientLogParser();
            LogParserBase middlewareParser = new MiddlewareLogParser();

            Match generalMatch = clientParser.GeneralLogRegex.Match(generalLog);
            Assert.IsTrue(generalMatch.Success);
            Assert.AreEqual("2018-10-29 16:35:32", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("651", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("客户端初始化应用程序相关属性", generalMatch.Groups["LogContent"].Value);

            generalMatch = serverParser.GeneralLogRegex.Match(ServerLog_0);
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

            generalMatch = serverParser.GeneralLogRegex.Match(ServerLog_1);
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

            generalMatch = serverParser.GeneralLogRegex.Match(ServerLog_2);
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

            generalMatch = serverParser.GeneralLogRegex.Match(ServerLog_3);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-12-11 20:28:12", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("140", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("中间件进程启动开始, 进程号[13800], 中间件启动", generalMatch.Groups["LogContent"].Value);
            serverMatch = serverParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            // 防止因为日志内容存在空格，而被正则错误分隔
            Assert.IsFalse(serverMatch.Success);

            generalMatch = clientParser.GeneralLogRegex.Match(ClientLog);
            Assert.IsTrue(generalMatch.Success);
            Assert.IsTrue(generalMatch.Groups["LogContent"].Success);
            Assert.AreEqual("2018-10-29 16:51:04", generalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("457", generalMatch.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", generalMatch.Groups["LogLevel"].Value);
            Assert.AreEqual("安信证券 1.3.0.065 192.168.7.101 初始化准备", generalMatch.Groups["LogContent"].Value);
            Match clientMatch = clientParser.ParticularRegex.Match(generalMatch.Groups["LogContent"].Value);
            Assert.IsTrue(clientMatch.Success);
            Assert.AreEqual("安信证券", clientMatch.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.065", clientMatch.Groups["Version"].Value);
            Assert.AreEqual("192.168.7.101", clientMatch.Groups["IPAddress"].Value);
            Assert.AreEqual("初始化准备", clientMatch.Groups["LogContent"].Value);

            Match middlewareMatch = middlewareParser.GeneralLogRegex.Match(MiddlewareLog);
            Assert.IsTrue(middlewareMatch.Success);
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