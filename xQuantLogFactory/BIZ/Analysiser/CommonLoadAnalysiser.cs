using System;
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
        /// Gets or sets 针对的监视规则名称
        /// </summary>
        public override string TargetMonitorName { get; set; }

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
            if (string.IsNullOrEmpty(this.TargetMonitorName))
            {
                throw new ArgumentNullException(nameof(this.TargetMonitorName));
            }

            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            argument.AnalysisResults
                .Where(result => result.MonitorItem.Name == this.TargetMonitorName)
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
                            childMonitorName = $"{this.TargetMonitorName}-{resourceName}";

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
                            // TODO: 耗时严重
                            // 匹配失败，删除无效的结果
                            LogDBContext.UnityDBContext.MonitorResults.Remove(monitorResult);
                            LogDBContext.UnityDBContext.AnalysisResults.Remove(analysisResult);

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
