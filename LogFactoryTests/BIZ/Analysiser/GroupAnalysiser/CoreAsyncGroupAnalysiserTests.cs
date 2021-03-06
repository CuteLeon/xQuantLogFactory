﻿using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using LogFactory.BIZ.Analysiser.GroupAnalysiser.Terminal;

namespace LogFactory.BIZ.Analysiser.GroupAnalysiser.Tests
{
    [TestClass()]
    public class CoreAsyncGroupAnalysiserTests
    {
        [TestMethod()]
        public void CoreAsyncGroupAnalysiserTest()
        {
            string[] testString = new string[] {
                "<-消息服务2355|执行->",
                "<-EXH_Collect23371|执行->",
                "<-交易接口服务659|执行触->",
                "<-指令轮询服务5617|结束:123->",
                "<-EXH_Collect23371|结束:0->",
                "<-外汇上行2.0对话报价服务5855|结束:8->",
            };
            AsyncGroupLogAnalysiserBase analysiser = new CoreAsyncGroupAnalysiser();
            Match match = null;

            Assert.IsTrue(testString.All(log => analysiser.AnalysisRegex.Match(log).Success));

            match = analysiser.AnalysisRegex.Match(testString[0]);
            Assert.AreEqual("消息服务", match.Groups["CoreServiceName"].Value);
            Assert.AreEqual("2355", match.Groups["Index"].Value);
            Assert.AreEqual("", match.Groups["Elapsed"].Value);
            Assert.AreEqual("", match.Groups["Trigger"].Value);

            match = analysiser.AnalysisRegex.Match(testString[1]);
            Assert.AreEqual("EXH_Collect", match.Groups["CoreServiceName"].Value);
            Assert.AreEqual("23371", match.Groups["Index"].Value);
            Assert.AreEqual("", match.Groups["Elapsed"].Value);
            Assert.AreEqual("", match.Groups["Trigger"].Value);

            match = analysiser.AnalysisRegex.Match(testString[2]);
            Assert.AreEqual("交易接口服务", match.Groups["CoreServiceName"].Value);
            Assert.AreEqual("659", match.Groups["Index"].Value);
            Assert.AreEqual("", match.Groups["Elapsed"].Value);
            Assert.AreEqual("触", match.Groups["Trigger"].Value);

            match = analysiser.AnalysisRegex.Match(testString[3]);
            Assert.AreEqual("指令轮询服务", match.Groups["CoreServiceName"].Value);
            Assert.AreEqual("5617", match.Groups["Index"].Value);
            Assert.AreEqual("123", match.Groups["Elapsed"].Value);
            Assert.AreEqual("", match.Groups["Trigger"].Value);

            match = analysiser.AnalysisRegex.Match(testString[4]);
            Assert.AreEqual("EXH_Collect", match.Groups["CoreServiceName"].Value);
            Assert.AreEqual("23371", match.Groups["Index"].Value);
            Assert.AreEqual("0", match.Groups["Elapsed"].Value);
            Assert.AreEqual("", match.Groups["Trigger"].Value);

            match = analysiser.AnalysisRegex.Match(testString[5]);
            Assert.AreEqual("外汇上行2.0对话报价服务", match.Groups["CoreServiceName"].Value);
            Assert.AreEqual("5855", match.Groups["Index"].Value);
            Assert.AreEqual("8", match.Groups["Elapsed"].Value);
            Assert.AreEqual("", match.Groups["Trigger"].Value);
        }
    }
}