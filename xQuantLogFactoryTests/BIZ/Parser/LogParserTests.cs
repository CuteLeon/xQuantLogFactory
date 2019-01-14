using System;
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

        [TestMethod]
        public void PerformanceStatisticalParseTest()
        {
            // 演示日志
            string log = @"2019-01-11 17:22:29.413	最近时段内方法执行统计信息：
方法=/LogonManager.soap>GetSysParamTable	计数=3	Max=9.9709	Min=7.9781	Avg=9.17806666666667	Sum=27.5342	Request=375	Response=99873
方法=/InitDataManager.soap>GetMarket	计数=1	Max=7.9764	Min=7.9764	Avg=7.9764	Sum=7.9764	Request=121	Response=1965
方法=/SimpleManagerMD.soap>Query	计数=5	Max=190.4891	Min=4.9875	Avg=55.46396	Sum=277.3198	Request=1614	Response=29056
方法=/InitDataManager.soap>GetServerBizDict_Str_Str	计数=3	Max=19.9515	Min=2.9918	Avg=14.2963666666667	Sum=42.8891	Request=436	Response=106980
方法=/InitDataManager.soap>GetIntSecuAccount	计数=1	Max=524.9089	Min=524.9089	Avg=524.9089	Sum=524.9089	Request=159	Response=112866
方法=/InitDataManager.soap>GetBizLineCode	计数=1	Max=0.9973	Min=0.9973	Avg=0.9973	Sum=0.9973	Request=123	Response=341
方法=/InitDataManager.soap>GetAccSecuMapping	计数=1	Max=48.8682	Min=48.8682	Avg=48.8682	Sum=48.8682	Request=128	Response=80080
共计 7 个方法。";

            // 正则1：获取日志时间和统计方法总数
            Regex statisticalRegex = new Regex(
                @"(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\t最近时段内方法执行统计信息：(.\r?\n?)*共计\s(?<MethodCount>\d*?)\s个方法。",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match statisticalMatch = statisticalRegex.Match(log);

            Assert.IsTrue(statisticalMatch.Success);
            Assert.AreEqual("2019-01-11 17:22:29.413", statisticalMatch.Groups["LogTime"].Value);
            Assert.AreEqual("7", statisticalMatch.Groups["MethodCount"].Value);

            // 正则2：获取每个方法的统计信息
            Regex methodRegex = new Regex(
                @"方法=(?<RequestURI>.*?)\>(?<MethodName>.*?)\t计数\=(?<Count>\d*)\tMax\=(?<Max>[\.\d]*)\tMin\=(?<Min>[\.\d]*)\tAvg\=(?<Avg>[\.\d]*)\tSum\=(?<Sum>[\.\d]*)\tRequest\=(?<Request>\d*)\tResponse\=(?<Response>\d*)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection methodMatchs = methodRegex.Matches(log);

            Assert.AreEqual(7, methodMatchs.Count);
            foreach (Match methodMatch in methodMatchs)
            {
                Assert.IsTrue(methodMatch.Success);

                Console.WriteLine(methodMatchs[0].Groups["RequestURI"].Value);
                Console.WriteLine(methodMatchs[0].Groups["MethodName"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Count"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Max"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Min"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Avg"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Sum"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Request"].Value);
                Console.WriteLine(methodMatchs[0].Groups["Response"].Value);
            }

            Assert.AreEqual("/LogonManager.soap", methodMatchs[0].Groups["RequestURI"].Value);
            Assert.AreEqual("GetSysParamTable", methodMatchs[0].Groups["MethodName"].Value);
            Assert.AreEqual("3", methodMatchs[0].Groups["Count"].Value);
            Assert.AreEqual("9.9709", methodMatchs[0].Groups["Max"].Value);
            Assert.AreEqual("7.9781", methodMatchs[0].Groups["Min"].Value);
            Assert.AreEqual("9.17806666666667", methodMatchs[0].Groups["Avg"].Value);
            Assert.AreEqual("27.5342", methodMatchs[0].Groups["Sum"].Value);
            Assert.AreEqual("375", methodMatchs[0].Groups["Request"].Value);
            Assert.AreEqual("99873", methodMatchs[0].Groups["Response"].Value);
        }
    }
}