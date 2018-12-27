using xQuantLogFactory.Model.Fixed;
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
    public abstract class MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : LogResultBase<TLogFile>
        where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets 监控项目
        /// </summary>
        public TMonitor MonitorItem { get; set; }

        /// <summary>
        /// Gets or sets iP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets 监视结果匹配模式
        /// </summary>
        public GroupTypes GroupType { get; set; }
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
