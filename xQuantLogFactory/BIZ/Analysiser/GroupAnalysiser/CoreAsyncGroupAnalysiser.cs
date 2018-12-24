using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// Core服务异步组分析器
    /// </summary>
    public class CoreAsyncGroupAnalysiser : AsyncGroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAsyncGroupAnalysiser"/> class.
        /// </summary>
        public CoreAsyncGroupAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAsyncGroupAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CoreAsyncGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析器正则表达式
        /// </summary>
        /// <remarks>
        /// 1. 服务名称禁止以数字结尾，否则会与 Index 混淆
        /// 2. Regex 必须附带 RegexOptions.RightToLeft 枚举值以同时应对服务名称中间包含的数字的日志内容
        /// </remarks>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"\<-(?<CoreServiceName>.*)(?<Index>\d*)\|(执行(?<Trigger>.*)|结束:(?<Elapsed>\d*))-\>",
            RegexOptions.RightToLeft | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

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

            this.Tracer?.WriteLine($"执行 Core服务异步组分析器 ....");
            argument.TerminalMonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == GroupAnalysiserTypes.CoreServiceAsync)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;

                    // 待匹配监视结果寄存字典
                    Dictionary<(string, int), TerminalAnalysisResult> unintactResults = new Dictionary<(string, int), TerminalAnalysisResult>();
                    TerminalAnalysisResult analysisResult = null;
                    Match analysisMatch = null;
                    string coreServiceName = string.Empty, elapsed = string.Empty, trigger = string.Empty;
                    int index = -1;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");
                    foreach (var monitorResult in resultGroup)
                    {
                        lock (this.lockSeed)
                        {
                            analysisMatch = this.AnalysisRegex.Match(monitorResult.LogContent);
                        }

                        coreServiceName = analysisMatch.Groups["CoreServiceName"].Value;
                        index = int.TryParse(analysisMatch.Groups["Index"].Value, out int value) ? value : -1;
                        trigger = analysisMatch.Groups["Trigger"].Value;
                        elapsed = analysisMatch.Groups["Elapsed"].Value;

                        // 获取寄存器内未关闭的分析结果
                        unintactResults.TryGetValue((coreServiceName, index), out analysisResult);

                        switch (monitorResult.GroupType)
                        {
                            case GroupTypes.Start:
                                {
                                    // 组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);

                                    analysisResult.AnalysisDatas[FixedDatas.CORE_SERVICE_NAME] = coreServiceName;
                                    analysisResult.AnalysisDatas[FixedDatas.EXECUTE_INDEX] = index;
                                    analysisResult.AnalysisDatas[FixedDatas.TRIGGER] = trigger.Equals("触", StringComparison.OrdinalIgnoreCase) ? FixedDatas.TRIGGER_ON : FixedDatas.TRIGGER_OFF;

                                    unintactResults[(coreServiceName, index)] = analysisResult;

                                    break;
                                }

                            case GroupTypes.Finish:
                                {
                                    if (analysisResult != null &&
                                        analysisResult.StartMonitorResult.CheckMatch(monitorResult))
                                    {
                                        // 组匹配类型为Finish时，若存在未关闭的分析结果且结果匹配，则组装分析结果并出队；
                                        analysisResult.FinishMonitorResult = monitorResult;

                                        unintactResults.Remove((coreServiceName, index));
                                    }
                                    else
                                    {
                                        // 不存在同服务名称且同执行序号的分析结果或分析结果不匹配时，新建分析结果
                                        analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);

                                        analysisResult.AnalysisDatas[FixedDatas.CORE_SERVICE_NAME] = coreServiceName;
                                        analysisResult.AnalysisDatas[FixedDatas.EXECUTE_INDEX] = index;
                                        analysisResult.AnalysisDatas[FixedDatas.TRIGGER] = trigger.Equals("触", StringComparison.OrdinalIgnoreCase) ? FixedDatas.TRIGGER_ON : FixedDatas.TRIGGER_OFF;
                                    }

                                    analysisResult.ElapsedMillisecond = double.TryParse(elapsed, out double elapsedValue) ? elapsedValue : double.NaN;
                                    break;
                                }
                        }

                        // Console.WriteLine($"设置分析数据：{coreServiceName}, {index}, {elapsed}; 寄存器数据：{unintactResults.Count} 个");
                    }

                    this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{targetMonitor.AnalysisResults.Count}\t监视规则：{targetMonitor.Name}");
                });
        }
    }
}
