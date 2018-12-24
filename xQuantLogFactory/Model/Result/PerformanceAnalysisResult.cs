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
        /// 绑定父分析结果节点
        /// </summary>
        /// <param name="monitorResult"></param>
        public override void BindMonitorResult(PerformanceMonitorResult monitorResult)
        {
            base.BindMonitorResult(monitorResult);
        }
    }
}
