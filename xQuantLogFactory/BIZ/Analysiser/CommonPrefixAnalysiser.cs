using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
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

            argument.AnalysisResults
                .Where(result => result.MonitorItem.Analysiser == AnalysiserTypes.Prefix)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;
                    MonitorItem childMonitor = null;
                    MonitorResult firstResult = null;
                    string customeData = string.Empty;

                    foreach (var analysisResult in resultGroup)
                    {
                        firstResult = analysisResult.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        customeData = firstResult.LogContent.Substring((firstResult.GroupType == GroupTypes.Finish ? targetMonitor.FinishPatterny : targetMonitor.StartPattern).Length).Trim();
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
