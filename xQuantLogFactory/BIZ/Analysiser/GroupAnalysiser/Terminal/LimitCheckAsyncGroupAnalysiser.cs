using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser.Terminal
{
    /// <summary>
    /// 限额检查异步组分析器
    /// </summary>
    public class LimitCheckAsyncGroupAnalysiser : GroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckAsyncGroupAnalysiser"/> class.
        /// </summary>
        public LimitCheckAsyncGroupAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckAsyncGroupAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public LimitCheckAsyncGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            // TODO: Excel 模板增加限额检查 Sheet
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 限额检查异步组分析器 ....");
            argument.TerminalMonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.LimitCheckAsync)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    var targetMonitor = resultGroup.Key;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");

                    foreach (var monitorResult in resultGroup)
                    {
                        var analysisResult = this.CreateTerminalAnalysisResult(argument, targetMonitor, monitorResult);

                        analysisResult.StartMonitorResult = analysisResult.FinishMonitorResult = monitorResult;
                    }

                    this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{targetMonitor.AnalysisResults.Count}\t监视规则：{targetMonitor.Name}");
                });
        }
    }
}
