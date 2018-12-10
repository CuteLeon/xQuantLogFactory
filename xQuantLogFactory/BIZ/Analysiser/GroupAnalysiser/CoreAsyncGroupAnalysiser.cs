﻿using System;
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
    public class CoreAsyncGroupAnalysiser : AsyncGroupLogAnalysiser
    {
        public CoreAsyncGroupAnalysiser()
        {
        }

        public CoreAsyncGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        // TODO: 实现自定义正则表达式
        public override Regex AnalysisRegex { get => base.AnalysisRegex; protected set => base.AnalysisRegex = value; }

        // TODO: 异步日志匹配
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            argument.MonitorResults
                .Where(result => result.MonitorItem.Async == true)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(monitorResultGroup =>
                {
                    if (!(monitorResultGroup.Key is MonitorItem monitor))
                    {
                        this.Tracer?.WriteLine("无法分析空的监视规则");
                        return;
                    }
                    this.Tracer?.WriteLine($"开始分析监视规则：{monitor.Name}");

                    GroupAnalysisResult analysisResult = null;
                    foreach (var monitorResult in monitorResultGroup.OrderBy(result => result.LogTime))
                    {
                        switch (monitorResult.GroupType)
                        {
                            case GroupTypes.Start:
                                {
                                    // 组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResult = this.CreateAnalysisResult(argument, monitor, monitorResult);
                                    break;
                                }
                            case GroupTypes.Finish:
                                {
                                    if (analysisResult == null ||
                                        !analysisResult.StartMonitorResult.CheckMatch(monitorResult))
                                    {
                                        // 组匹配类型为Finish时，若不存在未关闭的分析结果或结果不匹配，则新建分析结果并记录监视结果；
                                        analysisResult = this.CreateAnalysisResult(argument, monitor, monitorResult);
                                    }
                                    else
                                    {
                                        // 组匹配类型为Finish时，若存在未关闭的分析结果，则记录监视结果并计算结果耗时；
                                        analysisResult.FinishMonitorResult = monitorResult;

                                        if (analysisResult.StartMonitorResult != null &&
                                            analysisResult.StartMonitorResult.LogTime != null &&
                                            analysisResult.FinishMonitorResult != null &&
                                            analysisResult.FinishMonitorResult.LogTime != null)
                                        {
                                            analysisResult.ElapsedMillisecond = (analysisResult.FinishMonitorResult.LogTime - analysisResult.StartMonitorResult.LogTime).TotalMilliseconds;
                                        }
                                    }

                                    // 关闭分析结果，否则会影响下次状态判断
                                    analysisResult = null;
                                    break;
                                }
                        }
                    }
                });
        }
    }
}
