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
    /// 通用
    /// </summary>
    public class CommonMemoryAnalysiser : DirectedLogAnalysiserBase
    {
        public CommonMemoryAnalysiser()
        {
        }

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

            argument.AnalysisResults
                .Where(result => result.MonitorItem.Memory)
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
