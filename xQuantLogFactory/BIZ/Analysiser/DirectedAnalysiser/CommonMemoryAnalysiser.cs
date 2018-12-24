using System;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser
{
    /// <summary>
    /// 通用
    /// </summary>
    public class CommonMemoryAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMemoryAnalysiser"/> class.
        /// </summary>
        public CommonMemoryAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonMemoryAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonMemoryAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 内存日志内容正则
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"内存消耗：.*?VirtualMem=(?<Memory>\d*\.\d*).*",
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

            this.Tracer?.WriteLine($"执行 通用内存定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.Memory)
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

                        analysisResult.AnalysisDatas[FixedDatas.MEMORY_CONSUMED] =
                            analysisMatch.Success &&
                            analysisMatch.Groups["Memory"].Success &&
                            double.TryParse(analysisMatch.Groups["Memory"].Value, out double memory) ?
                                memory : 0.0;
                    }
                });
        }
    }
}
