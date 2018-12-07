using System;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser
{
    /// <summary>
    /// 交易清算分析器
    /// </summary>
    public class CommonKeyValuePairAnalysiser : DirectedLogAnalysiserBase
    {
        public CommonKeyValuePairAnalysiser()
        {
        }

        public CommonKeyValuePairAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            $@"\[.*?\]",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // TODO: 正则需要调试 [^|(,\s?)][,|=]*=.*[(,\s?)|$]
        public virtual Regex KeyValuePairRegex { get; protected set; } = new Regex(
            $@"((?<Key>.+?)=(?<Value>.*?))?(,\s?)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            argument.AnalysisResults
                .Where(result => result.MonitorItem.Analysiser == AnalysiserTypes.KeyValuePair)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;
                    MonitorResult firstResult = null;
                    Match analysisMatch = null;
                    string customeData = string.Empty;

                    foreach (var analysisResult in resultGroup)
                    {
                        firstResult = analysisResult.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        analysisMatch = this.AnalysisRegex.Match(firstResult.LogContent);
                        if (analysisMatch.Success)
                        {
                            if (analysisMatch.Groups["QSRQ"].Success)
                            {
                                analysisResult.AnalysisDatas[FixedDatas.QSRQ] = analysisMatch.Groups["QSRQ"].Value;
                            }
                            if (analysisMatch.Groups["QSJD"].Success)
                            {
                                analysisResult.AnalysisDatas[FixedDatas.QSJD] = analysisMatch.Groups["QSJD"].Value;
                            }
                            if (analysisMatch.Groups["DQZH"].Success)
                            {
                                analysisResult.AnalysisDatas[FixedDatas.DQZH] = analysisMatch.Groups["DQZH"].Value;
                            }
                            if (analysisMatch.Groups["WBZH"].Success)
                            {
                                analysisResult.AnalysisDatas[FixedDatas.WBZH] = analysisMatch.Groups["WBZH"].Value;
                            }
                            if (analysisMatch.Groups["ZQ"].Success)
                            {
                                analysisResult.AnalysisDatas[FixedDatas.ZQ] = analysisMatch.Groups["ZQ"].Value;
                            }
                        }
                    }
                });
        }
    }
}
