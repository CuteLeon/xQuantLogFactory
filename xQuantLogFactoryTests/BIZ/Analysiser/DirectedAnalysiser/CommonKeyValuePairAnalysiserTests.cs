using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Tests
{
    [TestClass()]
    public class CommonKeyValuePairAnalysiserTests
    {
        [TestMethod()]
        public void AnalysisTest()
        {
            /* TODO: 正则需要调试 [^|(,\s?)][,|=]*=.*[(,\s?)|$]
清算日期 = 2014-11-20, 清算进度=1/1,
当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys
,清算日期=2014-11-20, 清算进度=0/1
清算日期=2014-11-20, 清算进度=1/1,当前账户=zhuxx_nbzj_yhj2
清算日期=2014-11-20, 清算进度=1/1,当前账户=zhuxx_nbzj_yhj2
清算日期=2014-11-20, 清算进度=1/1,当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys
清算日期=2014-11-20,当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys
[当前账户=zhuxx_nbzj_yhj2,外部账户=zhuxx_wbzj_jys,债券=,,
             */
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

            foreach (string log in logs)
            {
                matchCollectiones.Add(analysiser.AnalysisRegex.Matches(log));
            }

            foreach (MatchCollection matchCollection in matchCollectiones)
            {
                Console.WriteLine($"> 匹配结果集合：{matchCollection.Count}——————————");
                foreach (Match match in matchCollection)
                {
                    Console.WriteLine($"> 匹配括号内容：{match.Value}");

                    MatchCollection keyValuePairMatchs = analysiser.KeyValuePairRegex.Matches(match.Value);

                    foreach (Match keyValuePairMatch in keyValuePairMatchs)
                    {
                        Console.WriteLine($"=> {keyValuePairMatch.Groups["Key"].Value}");
                        Console.WriteLine($"=> {keyValuePairMatch.Groups["Value"].Value}");
                    }
                }
            }
        }
    }
}