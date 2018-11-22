using System;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 通用加载分析器
    /// </summary>
    public class CommonLoadAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// 针对的监视规则名称
        /// </summary>
        public override string TargetMonitorName { get; set; }

        /// <summary>
        /// 加载日志内容正则
        /// </summary>
        public Regex AnalysisRegex { get; protected set; } = new Regex(
            @"^加载(?<ResourceName>[a-zA-Z_]+)：(?<Elapsed>\d*)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

        public CommonLoadAnalysiser() { }

        public CommonLoadAnalysiser(ITracer tracer) : base(tracer) { }

        /// <summary>
        /// 分析监视内容作为前缀的操作日志
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (string.IsNullOrEmpty(this.TargetMonitorName))
                throw new ArgumentNullException(nameof(this.TargetMonitorName));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            argument.AnalysisResults
                .Where(result => result.MonitorItem.Name == this.TargetMonitorName)
                .GroupBy(result => (/*result.LogFile,*/ result.MonitorItem))
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key/*.MonitorItem*/;
                    MonitorItem childMonitor = null;
                    Match logMatch = null;
                    string resourceName = string.Empty;
                    string childMonitorName = string.Empty;

                    foreach (var result in resultGroup)
                    {
                        if (result.StartMonitorResult == null) continue;
                        if (result.FinishMonitorResult != null) continue;

                        logMatch = this.AnalysisRegex.Match(result.StartMonitorResult.LogContent);
                        if (logMatch.Success && logMatch.Groups["ResourceName"].Success)
                        {
                            resourceName = logMatch.Groups["ResourceName"].Value;
                            childMonitorName = $"{this.TargetMonitorName}-{resourceName}";

                            childMonitor = this.TryGetOrAddChildMonitor(targetMonitor, childMonitorName);

                            if (logMatch.Groups["Elapsed"].Success &&
                                double.TryParse(logMatch.Groups["Elapsed"].Value, out double elapsed))
                            {
                                //构建完整组
                                result.FinishMonitorResult = result.StartMonitorResult;
                                result.ElapsedMillisecond = elapsed;
                            }

                            childMonitor.AnalysisResults.Add(result);
                            //childMonitor.MonitorResults.Add(result.StartMonitorResult);
                            //targetMonitor.MonitorResults.Remove(result.StartMonitorResult);
                            result.MonitorItem = childMonitor;
                            result.StartMonitorResult.MonitorItem = childMonitor;
                        }
                        /* TODO: 无效的结果在落实数据库时无法正常删除
                        if (result.StartMonitorResult != null)
                        {
                            result.StartMonitorResult.LogFile = null;
                            result.StartMonitorResult.TaskArgument = null;
                            result.StartMonitorResult.MonitorItem = null;
                            result.StartMonitorResult.MonitorItem = null;
                            targetMonitor.MonitorResults.Remove(result.StartMonitorResult);
                            argument.MonitorResults.Remove(result.StartMonitorResult);
                        }
                        if (result.FinishMonitorResult != null)
                        {
                            result.FinishMonitorResult.LogFile = null;
                            result.FinishMonitorResult.TaskArgument = null;
                            result.FinishMonitorResult.MonitorItem = null;
                            targetMonitor.MonitorResults.Remove(result.FinishMonitorResult);
                            argument.MonitorResults.Remove(result.FinishMonitorResult);
                        }
                        result.TaskArgument = null;
                        result.MonitorItem = null;
                        result.LogFile = null;
                        argument.AnalysisResults.Remove(result);
                         */
                        targetMonitor.AnalysisResults.Remove(result);
                    }
                });
        }

    }
}
