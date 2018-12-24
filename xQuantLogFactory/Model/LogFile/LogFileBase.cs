using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.LogFile
{
    /// <summary>
    /// 日志文件基类
    /// </summary>
    public class LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
    {

        /// <summary>
        /// Gets or sets 日志文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件相对路径
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件类型
        /// </summary>
        public LogFileTypes LogFileType { get; set; }

        /// <summary>
        /// Gets or sets 文件创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets 上次写入时间
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets 客户端和服务端匹配结果总耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond
        {
            get
            {
                return this.AnalysisResults.Sum(result => result.ElapsedMillisecond);
            }
        }

        /// <summary>
        /// Gets or sets 监视日志解析结果列表
        /// </summary>
        public virtual List<TMonitorResult> MonitorResults { get; set; } = new List<TMonitorResult>();

        /// <summary>
        /// Gets or sets 未解析日志结果列表
        /// </summary>
        public virtual List<TerminalUnparsedResult> UnparsedResults { get; set; } = new List<TerminalUnparsedResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
        /// </summary>
        public virtual List<TAnalysisResult> AnalysisResults { get; set; } = new List<TAnalysisResult>();
    }
}
