using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// 自封闭组分析器
    /// </summary>
    public class CommonSelfSealingGroupAnalysiser : GroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSelfSealingGroupAnalysiser"/> class.
        /// </summary>
        public CommonSelfSealingGroupAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSelfSealingGroupAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonSelfSealingGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

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

            this.Tracer?.WriteLine($"执行 通用自封闭组分析器 ....");
            argument.TerminalMonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == GroupAnalysiserTypes.SelfSealing)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");

                    foreach (var monitorResult in resultGroup)
                    {
                        TerminalAnalysisResult analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);

                        analysisResult.StartMonitorResult = analysisResult.FinishMonitorResult = monitorResult;
                    }

                    this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{targetMonitor.AnalysisResults.Count}\t监视规则：{targetMonitor.Name}");
                });
        }
    }
}
