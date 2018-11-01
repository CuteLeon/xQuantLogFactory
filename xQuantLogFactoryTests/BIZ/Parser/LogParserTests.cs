using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.BIZ.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace xQuantLogFactory.BIZ.Parser.Tests
{
    [TestClass()]
    public class LogParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            string[] logs = new string[]
            {
                "2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券",
                "2018-10-29 16:51:04,457 TRACE 安信证券 1.3.0.065 192.168.7.101 初始化准备",
                "2018-10-29 16:35:32,651 TRACE 客户端初始化应用程序相关属性",
            };
            ServerLogParser parser = new ServerLogParser();

            Match match_0 = parser.LogRegex.Match(logs[0]);
            Match match_1 = parser.LogRegex.Match(logs[1]);
            Match match_2 = parser.LogRegex.Match(logs[2]);

            Assert.IsTrue(match_0.Success);
            Assert.AreEqual("2018-10-30 09:25:30", match_0.Groups["LogTime"].Value);
            Assert.AreEqual("111", match_0.Groups["Millisecond"].Value);
            Assert.AreEqual("DEBUG", match_0.Groups["LogLevel"].Value);
            Assert.AreEqual("东方证券", match_0.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.064补丁1", match_0.Groups["Version"].Value);
            Assert.AreEqual("", match_0.Groups["IPAddress"].Value);
            Assert.AreEqual("开始排券", match_0.Groups["LogContent"].Value);

            Assert.IsTrue(match_1.Success);
            Assert.AreEqual("2018-10-29 16:51:04", match_1.Groups["LogTime"].Value);
            Assert.AreEqual("457", match_1.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", match_1.Groups["LogLevel"].Value);
            Assert.AreEqual("安信证券", match_1.Groups["Client"].Value);
            Assert.AreEqual("1.3.0.065", match_1.Groups["Version"].Value);
            Assert.AreEqual("192.168.7.101", match_1.Groups["IPAddress"].Value);
            Assert.AreEqual("初始化准备", match_1.Groups["LogContent"].Value);

            Assert.IsTrue(match_2.Success);
            Assert.AreEqual("2018-10-29 16:35:32", match_2.Groups["LogTime"].Value);
            Assert.AreEqual("651", match_2.Groups["Millisecond"].Value);
            Assert.AreEqual("TRACE", match_2.Groups["LogLevel"].Value);
            Assert.AreEqual("", match_2.Groups["Client"].Value);
            Assert.AreEqual("", match_2.Groups["Version"].Value);
            Assert.AreEqual("", match_2.Groups["IPAddress"].Value);
            Assert.AreEqual("客户端初始化应用程序相关属性", match_2.Groups["LogContent"].Value);
        }
    }
}