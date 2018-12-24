using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.Fixed;
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
    public abstract class LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : FileBase
        where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets 日志文件类型
        /// </summary>
        public LogFileTypes LogFileType { get; set; }

        /// <summary>
        /// Gets 分析结果总耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond
        {
            get
            {
                return this.AnalysisResults.Sum(result => result.ElapsedMillisecond);
            }
        }
        #endregion

        #region 泛型属性

        /// <summary>
        /// Gets or sets 日志解析结果列表
        /// </summary>
        public virtual List<TMonitorResult> MonitorResults { get; set; } = new List<TMonitorResult>();

        /// <summary>
        /// Gets or sets 日志分析结果列表
        /// </summary>
        public virtual List<TAnalysisResult> AnalysisResults { get; set; } = new List<TAnalysisResult>();
        #endregion
    }
}
