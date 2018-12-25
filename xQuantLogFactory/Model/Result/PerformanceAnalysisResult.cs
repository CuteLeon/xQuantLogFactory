using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// Performance分析结果
    /// </summary>
    public class PerformanceAnalysisResult : AnalysisResultBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceAnalysisResult"/> class.
        /// </summary>
        public PerformanceAnalysisResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceAnalysisResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="monitor"></param>
        /// <param name="logFile"></param>
        public PerformanceAnalysisResult(TaskArgument argument, PerformanceMonitorItem monitor, PerformanceLogFile logFile)
        {
            this.LogFile = logFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;
        }

        /// <summary>
        /// 绑定父分析结果节点
        /// </summary>
        /// <param name="monitorResult"></param>
        public override void BindMonitorResult(PerformanceMonitorResult monitorResult)
        {
            base.BindMonitorResult(monitorResult);
        }
    }
}
