﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.DAL;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 通用加载分析器
    /// </summary>
    public class CommonLoadAnalysiser : DirectedLogAnalysiserBase
    {
        public CommonLoadAnalysiser()
        {
        }

        public CommonLoadAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 加载日志内容正则
        /// </summary>
        public Regex AnalysisRegex { get; protected set; } = new Regex(
            @"^加载(?<ResourceName>[a-zA-Z_]+)：(?<Elapsed>\d*)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

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

            //TODO: 改写为搜索 监视规则 并按监视规则分组，处理监视规则的分析结果
            argument.AnalysisResults
                .Where(result => result.MonitorItem.Analysiser == AnalysiserTypes.Load)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;
                    MonitorItem childMonitor = null;
                    MonitorResult monitorResult = null;
                    Match logMatch = null;
                    string resourceName = string.Empty;
                    string childMonitorName = string.Empty;

                    foreach (var analysisResult in resultGroup)
                    {
                        if (analysisResult.StartMonitorResult == null)
                        {
                            continue;
                        }

                        if (analysisResult.FinishMonitorResult != null)
                        {
                            continue;
                        }

                        monitorResult = analysisResult.StartMonitorResult;
                        logMatch = this.AnalysisRegex.Match(monitorResult.LogContent);
                        if (logMatch.Success && logMatch.Groups["ResourceName"].Success)
                        {
                            resourceName = logMatch.Groups["ResourceName"].Value;
                            childMonitorName = $"{targetMonitor.Name}-{resourceName}";

                            childMonitor = this.TryGetOrAddChildMonitor(targetMonitor, childMonitorName);

                            if (logMatch.Groups["Elapsed"].Success &&
                                double.TryParse(logMatch.Groups["Elapsed"].Value, out double elapsed))
                            {
                                // 构建完整组
                                analysisResult.FinishMonitorResult = monitorResult;
                                analysisResult.ElapsedMillisecond = elapsed;
                            }

                            // 匹配成功，迁移结果到子监视规则
                            analysisResult.MonitorItem = childMonitor;
                            childMonitor.AnalysisResults.Add(analysisResult);
                            targetMonitor.AnalysisResults.Remove(analysisResult);

                            monitorResult.MonitorItem = childMonitor;
                            childMonitor.MonitorResults.Add(monitorResult);
                            targetMonitor.MonitorResults.Remove(monitorResult);
                        }
                        else
                        {
                            // 匹配失败，删除无效的结果
                            /* TODO: [ORM] 耗时严重
                             * 分析器 Host 增加前序分析器列表，此列表内的分析器将早于 Host 分析算法执行，并将处理后的监视结果标记状态，Host 不再处理被标记的监视结果
                             * 但仍需删除无效的监视结果，是否耗时严重？
                             */
                            /*
                            LogDBContext.UnityDBContext.MonitorResults.Remove(monitorResult);
                            LogDBContext.UnityDBContext.AnalysisResults.Remove(analysisResult);
                             */

                            /*
                            monitorResult.TaskArgument.MonitorResults.Remove(monitorResult);
                            monitorResult.MonitorItem.MonitorResults.Remove(monitorResult);
                            monitorResult.LogFile.MonitorResults.Remove(monitorResult);
                            monitorResult.TaskArgument = null;
                            monitorResult.MonitorItem = null;
                            monitorResult.LogFile = null;

                            analysisResult.TaskArgument.AnalysisResults.Remove(analysisResult);
                            analysisResult.MonitorItem.AnalysisResults.Remove(analysisResult);
                            analysisResult.LogFile.AnalysisResults.Remove(analysisResult);
                            analysisResult.TaskArgument = null;
                            analysisResult.MonitorItem = null;
                            analysisResult.LogFile = null;
                             */
                        }
                    }
                });
        }
    }
}
