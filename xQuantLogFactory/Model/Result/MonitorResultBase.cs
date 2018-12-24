using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile> : LogResultBase
        where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {
        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        public TLogFile LogFile { get; set; }
    }
}
