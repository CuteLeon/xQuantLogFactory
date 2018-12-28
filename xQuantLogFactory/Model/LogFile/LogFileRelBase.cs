using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.LogFile
{
    /// <summary>
    /// 日志文件基类
    /// </summary>
    /// <typeparam name="TMonitor"></typeparam>
    /// <typeparam name="TMonitorResult"></typeparam>
    /// <typeparam name="TAnalysisResult"></typeparam>
    /// <typeparam name="TLogFile"></typeparam>
    public abstract class LogFileRelBase<TMonitorResult, TAnalysisResult, TLogFile> : LogFileBase
        where TMonitorResult : MonitorResultBase<TLogFile>
        where TAnalysisResult : AnalysisResultBase<TLogFile>
        where TLogFile : LogFileBase
    {
        #region 泛型属性

        /// <summary>
        /// Gets 分析结果总耗时（单位：毫秒）
        /// </summary>
        public override double ElapsedMillisecond
        {
            get => this.AnalysisResults.Sum(result => result.ElapsedMillisecond);
        }

        /// <summary>
        /// Gets or sets 日志监视结果列表
        /// </summary>
        public virtual List<TMonitorResult> MonitorResults { get; set; } = new List<TMonitorResult>();

        /// <summary>
        /// Gets or sets 日志分析结果列表
        /// </summary>
        public virtual List<TAnalysisResult> AnalysisResults { get; set; } = new List<TAnalysisResult>();
        #endregion
    }
}
