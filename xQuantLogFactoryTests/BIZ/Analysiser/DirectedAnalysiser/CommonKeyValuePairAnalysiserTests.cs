using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Tests
{
    [TestClass()]
    public class CommonKeyValuePairAnalysiserTests
    {
        [TestMethod()]
        public void AnalysisTest()
        {
            string[] logs = new string[]
            {
                "日终清算[清算日期=2014-11-20, 清算进度=1/1,][当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys] 开始------",
                "日终清算[,清算日期=2014-11-20, 清算进度=0/1] 开始------",
                "日志信息: 时间=2018-11-21 17:14:56,用户=ww,信息类型=DEBUG,来源=基础管理,摘要=日终清算,明细=日终清算[清算日期=2014-11-20, 清算进度=1/1,当前账户=zhuxx_nbzj_yhj2] 开始------",
                "日终清算[清算日期=2014-11-20, 清算进度=1/1,当前账户=zhuxx_nbzj_yhj2] 准备金融工具现金流------",
                "日终清算[清算日期=2014-11-20, 清算进度=1/1,当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys] 开始------",
                "....日终清算[清算日期=2014-11-20,当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys] 清算前准备初始化完成;",
                "[当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys,债券=,,] 刷新分销额度获取已有额度开始;"
            };
            List<MatchCollection> matchCollectiones = new List<MatchCollection>();
            CommonKeyValuePairAnalysiser analysiser = new CommonKeyValuePairAnalysiser();
            List<Dictionary<string, string>> matchDictionaries = new List<Dictionary<string, string>>();

            foreach (string log in logs)
            {
                matchCollectiones.Add(analysiser.AnalysisRegex.Matches(log));
            }

            foreach (MatchCollection matchCollection in matchCollectiones)
            {
                Console.WriteLine($"> 匹配结果集合：{matchCollection.Count}——————————");
                Dictionary<string, string> matchDictionary = new Dictionary<string, string>();
                matchDictionaries.Add(matchDictionary);

                foreach (Match match in matchCollection)
                {
                    Console.WriteLine($"> 匹配括号内容：{match.Groups["Pairs"].Value}");

                    MatchCollection keyValuePairMatchs = analysiser.KeyValuePairRegex.Matches(match.Groups["Pairs"].Value);
                    foreach (Match keyValuePairMatch in keyValuePairMatchs)
                    {
                        Console.WriteLine($"Key =>{keyValuePairMatch.Groups["Key"].Success} => {keyValuePairMatch.Groups["Key"].Value}");
                        Console.WriteLine($"Value => {keyValuePairMatch.Groups["Value"].Success} => {keyValuePairMatch.Groups["Value"].Value}");
                        matchDictionary[keyValuePairMatch.Groups["Key"].Value] = keyValuePairMatch.Groups["Value"].Value;
                    }
                    Console.WriteLine($"键值对个数：{matchDictionary.Count}");
                }
            }

            Assert.AreEqual(7, matchDictionaries.Count);
            Assert.AreEqual(4, matchDictionaries[0].Count);
            Assert.AreEqual(2, matchDictionaries[1].Count);
            Assert.AreEqual(3, matchDictionaries[2].Count);
            Assert.AreEqual(3, matchDictionaries[3].Count);
            Assert.AreEqual(4, matchDictionaries[4].Count);
            Assert.AreEqual(3, matchDictionaries[5].Count);
            Assert.AreEqual(3, matchDictionaries[6].Count);

            Assert.AreEqual("2014-11-20", matchDictionaries[0]["清算日期"]);
            Assert.AreEqual("1/1", matchDictionaries[0]["清算进度"]);
            Assert.AreEqual("zhuxx_wbzj_jys", matchDictionaries[0]["外部账户"]);
            Assert.AreEqual("zhuxx_nbzj_yhj2", matchDictionaries[0]["当前账户"]);

            Assert.AreEqual("2014-11-20", matchDictionaries[1]["清算日期"]);
            Assert.AreEqual("0/1", matchDictionaries[1]["清算进度"]);

            Assert.AreEqual("2014-11-20", matchDictionaries[2]["清算日期"]);
            Assert.AreEqual("1/1", matchDictionaries[2]["清算进度"]);
            Assert.AreEqual("zhuxx_nbzj_yhj2", matchDictionaries[2]["当前账户"]);

            Assert.AreEqual("2014-11-20", matchDictionaries[3]["清算日期"]);
            Assert.AreEqual("1/1", matchDictionaries[3]["清算进度"]);
            Assert.AreEqual("zhuxx_nbzj_yhj2", matchDictionaries[3]["当前账户"]);

            Assert.AreEqual("2014-11-20", matchDictionaries[4]["清算日期"]);
            Assert.AreEqual("1/1", matchDictionaries[4]["清算进度"]);
            Assert.AreEqual("zhuxx_wbzj_jys", matchDictionaries[4]["外部账户"]);
            Assert.AreEqual("zhuxx_nbzj_yhj2", matchDictionaries[4]["当前账户"]);

            Assert.AreEqual("2014-11-20", matchDictionaries[5]["清算日期"]);
            Assert.AreEqual("zhuxx_wbzj_jys", matchDictionaries[5]["外部账户"]);
            Assert.AreEqual("zhuxx_nbzj_yhj2", matchDictionaries[5]["当前账户"]);

            Assert.AreEqual("zhuxx_nbzj_yhj2", matchDictionaries[6]["当前账户"]);
            Assert.AreEqual("zhuxx_wbzj_jys", matchDictionaries[6]["外部账户"]);
            Assert.AreEqual(string.Empty, matchDictionaries[6]["债券"]);
        }
    }
}