using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 通用前缀分析器
    /// </summary>
    public class CommonPrefixAnalysiser : DirectedLogAnalysiserBase
    {
        public CommonPrefixAnalysiser()
        {
        }

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

            //TODO: 改写为搜索 监视规则 并按监视规则分组，处理监视规则的分析结果
            argument.AnalysisResults
                .Where(result => result.MonitorItem.Analysiser == AnalysiserTypes.Prefix)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;
                    MonitorItem childMonitor = null;
                    MonitorResult firstResult = null;
                    string customeData = string.Empty;
                    string childMonitorName = string.Empty;

                    foreach (var result in resultGroup)
                    {
                        firstResult = result.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        customeData = firstResult.LogContent.Substring((firstResult.GroupType == GroupTypes.Finish ? targetMonitor.FinishPatterny : targetMonitor.StartPattern).Length);
                        childMonitorName = $"{targetMonitor.Name}-{customeData}";

                        childMonitor = this.TryGetOrAddChildMonitor(targetMonitor, childMonitorName);

                        targetMonitor.AnalysisResults.Remove(result);
                        childMonitor.AnalysisResults.Add(result);
                        result.MonitorItem = childMonitor;

                        if (result.StartMonitorResult != null)
                        {
                            result.StartMonitorResult.MonitorItem = childMonitor;

                            // TODO [ORM] EF6框架帮我们完成了这部分
                            // childMonitor.MonitorResults.Add(result.StartMonitorResult);
                            // targetMonitor.MonitorResults.Remove(result.StartMonitorResult);
                        }
                        if (result.FinishMonitorResult != null)
                        {
                            result.FinishMonitorResult.MonitorItem = childMonitor;

                            // TODO [ORM] EF6框架帮我们完成了这部分
                            // childMonitor.MonitorResults.Add(result.FinishMonitorResult);
                            // targetMonitor.MonitorResults.Remove(result.FinishMonitorResult);
                        }
                    }
                });
        }
    }
}
