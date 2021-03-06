﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.GroupAnalysiser.Terminal
{
    /// <summary>
    /// 通用前缀异步组分析器
    /// </summary>
    /// <remarks>此组分析器会自动完成键值对分析，不需要再指定 DirectedAnalysiser="KeyValuePair"</remarks>
    public class ReportAsyncGroupAnalysiser : AsyncGroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAsyncGroupAnalysiser"/> class.
        /// </summary>
        public ReportAsyncGroupAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAsyncGroupAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ReportAsyncGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"(开始|完成)查询报表\[报表代码=(?<Code>.*?),报表名称=(?<Name>.*?),查询参数=(?<Param>.*?)\]",
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

            this.Tracer?.WriteLine($"执行 报表异步组分析器 ....");
            argument.TerminalMonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.ReportAsync)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;

                    // 待匹配监视结果寄存字典
                    Dictionary<(string, string), TerminalAnalysisResult> unintactResults = new Dictionary<(string, string), TerminalAnalysisResult>();
                    TerminalAnalysisResult analysisResult = null;
                    Match analysisMatch = null;
                    string reportName = string.Empty, reportCode = string.Empty, queryParam = string.Empty;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");
                    foreach (var monitorResult in resultGroup)
                    {
                        lock (this.lockSeed)
                        {
                            analysisMatch = this.AnalysisRegex.Match(monitorResult.LogContent);
                        }

                        reportName = analysisMatch.Groups["Name"].Value;
                        reportCode = analysisMatch.Groups["Code"].Value;
                        queryParam = analysisMatch.Groups["Param"].Value;

                        // 获取寄存器内未关闭的分析结果
                        unintactResults.TryGetValue((reportName, reportCode), out analysisResult);

                        switch (monitorResult.GroupType)
                        {
                            case GroupTypes.Start:
                                {
                                    // 组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResult = this.CreateTerminalAnalysisResult(argument, targetMonitor, monitorResult);

                                    analysisResult.AnalysisDatas[FixedDatas.REPORT_NAME] = reportName;
                                    analysisResult.AnalysisDatas[FixedDatas.REPORT_CODE] = reportCode;
                                    analysisResult.AnalysisDatas[FixedDatas.QUERY_PARAM] = queryParam;

                                    unintactResults[(reportName, reportCode)] = analysisResult;

                                    break;
                                }

                            case GroupTypes.Finish:
                                {
                                    if (analysisResult != null &&
                                        analysisResult.StartMonitorResult.CheckMatch(monitorResult))
                                    {
                                        // 组匹配类型为Finish时，若存在未关闭的分析结果且结果匹配，则组装分析结果并出队；
                                        analysisResult.FinishMonitorResult = monitorResult;

                                        unintactResults.Remove((reportName, reportCode));
                                    }
                                    else
                                    {
                                        // 不存在同服务名称且同执行序号的分析结果或分析结果不匹配时，新建分析结果
                                        analysisResult = this.CreateTerminalAnalysisResult(argument, targetMonitor, monitorResult);

                                        analysisResult.AnalysisDatas[FixedDatas.REPORT_NAME] = reportName;
                                        analysisResult.AnalysisDatas[FixedDatas.REPORT_CODE] = reportCode;
                                        analysisResult.AnalysisDatas[FixedDatas.QUERY_PARAM] = queryParam;
                                    }

                                    analysisResult.CalcElapsedMillisecond();
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
