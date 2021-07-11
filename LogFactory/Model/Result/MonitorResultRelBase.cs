using LogFactory.Model.LogFile;
using LogFactory.Model.Monitor;

namespace LogFactory.Model.Result
{
    /// <summary>
    /// 监视结果基类
    /// </summary>
    /// <typeparam name="TMonitor"></typeparam>
    /// <typeparam name="TMonitorResult"></typeparam>
    /// <typeparam name="TLogFile"></typeparam>
    public abstract class MonitorResultRelBase<TMonitor, TMonitorResult, TLogFile> : MonitorResultBase<TLogFile>
        where TMonitor : MonitorItemBase
        where TMonitorResult : MonitorResultBase<TLogFile>
        where TLogFile : LogFileBase
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
