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
    /// 通用日志组分析器
    /// </summary>
    public class CommonGroupLogAnalysiser : GroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonGroupLogAnalysiser"/> class.
        /// </summary>
        public CommonGroupLogAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonGroupLogAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonGroupLogAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 分析日志
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 通用组分析器 ....");
            argument.TerminalMonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.Common)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
            {
                TerminalMonitorItem targetMonitor = resultGroup.Key;
                TerminalAnalysisResult analysisResult = null;

                this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");
                foreach (var monitorResult in resultGroup)
                {
                    switch (monitorResult.GroupType)
                    {
                        case GroupTypes.Start:
                            {
                                // 组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);
                                break;
                            }

                        case GroupTypes.Finish:
                            {
                                if (analysisResult == null ||
                                    !analysisResult.StartMonitorResult.CheckMatch(monitorResult))
                                {
                                    // 组匹配类型为Finish时，若不存在未关闭的分析结果或结果不匹配，则新建分析结果并记录监视结果；
                                    analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);
                                }
                                else
                                {
                                    // 组匹配类型为Finish时，若存在未关闭的分析结果，则记录监视结果并计算结果耗时；
                                    analysisResult.FinishMonitorResult = monitorResult;

                                    analysisResult.CalcElapsedMillisecond();
                                }

                                // 关闭分析结果，否则会影响下次状态判断
                                analysisResult = null;
                                break;
                            }
                    }
                }

                this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{targetMonitor.AnalysisResults.Count}\t监视规则：{targetMonitor.Name}");
            });
        }
    }
}
