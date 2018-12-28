using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 监视结果基类
    /// </summary>
    /// <typeparam name="TMonitor"></typeparam>
    /// <typeparam name="TMonitorResult"></typeparam>
    /// <typeparam name="TAnalysisResult"></typeparam>
    /// <typeparam name="TLogFile"></typeparam>
    public abstract class MonitorResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : MonitorResultBase<TLogFile>
        where TMonitor : MonitorItemRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileRelBase<TMonitorResult, TAnalysisResult, TLogFile>
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets 监控项目
        /// </summary>
        public TMonitor MonitorItem { get; set; }
        #endregion

        #region 业务

        /// <summary>
        /// 检查两个监视结果是否匹配
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        public abstract bool CheckMatch(TMonitorResult targetResult);
        #endregion
    }
}
