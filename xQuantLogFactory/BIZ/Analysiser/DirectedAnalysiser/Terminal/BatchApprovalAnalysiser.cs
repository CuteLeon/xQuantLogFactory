using System;
using System.Linq;
using System.Text.RegularExpressions;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// 批量审批定向分析器
    /// </summary>
    public class BatchApprovalAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchApprovalAnalysiser"/> class.
        /// </summary>
        public BatchApprovalAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchApprovalAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public BatchApprovalAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则表达式
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"批量审批数据处理(开始|结束，耗时:\d*ms)，操作人:(?<UserCode>.*)$",
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

            this.Tracer?.WriteLine($"执行 批量审批定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.BatchApproval)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;
                    TerminalMonitorResult firstResult = null;
                    Match analysisMatch = null;
                    string customeData = string.Empty;

                    this.Tracer?.WriteLine($">>>正在分析监视规则：{targetMonitor.Name}，结果数量：{resultGroup.Count()}");
                    foreach (var analysisResult in resultGroup)
                    {
                        // 构建自封闭的完整分析结果
                        analysisResult.FinishMonitorResult = analysisResult.StartMonitorResult = firstResult = analysisResult.FirstResultOrDefault();

                        if (firstResult == null)
                        {
                            continue;
                        }

                        lock (this.AnalysisRegex)
                        {
                            analysisMatch = this.AnalysisRegex.Match(firstResult.LogContent);
                        }

                        analysisResult.AnalysisDatas[FixedDatas.USER_CODE] =
                            analysisMatch.Success && analysisMatch.Groups["UserCode"].Success ?
                            analysisMatch.Groups["UserCode"].Value :
                            string.Empty;
                    }
                });
        }
    }
}
