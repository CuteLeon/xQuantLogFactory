﻿using System;
using System.Linq;

using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// 通用前缀分析器
    /// </summary>
    public class CommonPrefixAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPrefixAnalysiser"/> class.
        /// </summary>
        public CommonPrefixAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPrefixAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonPrefixAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 分析监视内容作为前缀的操作日志
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 通用前缀定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.Prefix)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;
                    TerminalMonitorItem childMonitor = null;
                    TerminalMonitorResult firstResult = null;
                    string customeData = string.Empty;
                    string pattern = string.Empty;
                    int customeDataIndex = 0;

                    this.Tracer?.WriteLine($">>>正在分析监视规则：{targetMonitor.Name}，结果数量：{resultGroup.Count()}");
                    foreach (var analysisResult in resultGroup)
                    {
                        firstResult = analysisResult.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        // 判断日志内容是否以监视规则的条件为开头，true：在监视规则中移除开始的条件字符串；false：直接使用日志内容作为子监视规则名称
                        pattern = firstResult.GroupType == GroupTypes.Finish ? targetMonitor.FinishPattern : targetMonitor.StartPattern;
                        customeDataIndex = firstResult.LogContent.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
                        if (customeDataIndex > -1)
                        {
                            customeData = firstResult.LogContent.Substring(customeDataIndex + pattern.Length).Trim();
                        }
                        else
                        {
                            customeData = firstResult.LogContent;
                        }

                        childMonitor = this.TryGetOrAddChildMonitor(targetMonitor, customeData);

                        targetMonitor.AnalysisResults.Remove(analysisResult);
                        childMonitor.AnalysisResults.Add(analysisResult);
                        analysisResult.MonitorItem = childMonitor;

                        if (analysisResult.StartMonitorResult != null)
                        {
                            analysisResult.StartMonitorResult.MonitorItem = childMonitor;
                            childMonitor.MonitorResults.Add(analysisResult.StartMonitorResult);
                            targetMonitor.MonitorResults.Remove(analysisResult.StartMonitorResult);
                        }

                        if (analysisResult.FinishMonitorResult != null)
                        {
                            analysisResult.FinishMonitorResult.MonitorItem = childMonitor;
                            childMonitor.MonitorResults.Add(analysisResult.FinishMonitorResult);
                            targetMonitor.MonitorResults.Remove(analysisResult.FinishMonitorResult);
                        }
                    }
                });
        }
    }
}
