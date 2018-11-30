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
    public class TradeSettleAnalysiser : DirectedLogAnalysiserBase
    {
        public TradeSettleAnalysiser()
        {
        }

        public TradeSettleAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            $@"\[({FixedDatas.QSRQ}=(?<QSRQ>[\d-]{{10}}))?(,\s?)?({FixedDatas.QSJD}=(?<QSJD>[\d\/]*))?(,\s?)?({FixedDatas.DQZH}=(?<DQZH>[0-9a-zA-Z_]*))?(,\s?)?({FixedDatas.WBZH}=(?<WBZH>[0-9a-zA-Z_]*))?(,\s?)?({FixedDatas.ZQ}=(?<ZQ>.*?))?(,\s?)?(,\s?)?\]",
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
                .Where(result => result.MonitorItem.Analysiser == AnalysiserTypes.Settle)
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
