using System;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 中间件日志结果
    /// </summary>
    public class PerformanceMonitorResult : MonitorResultBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
    {
        public PerformanceMonitorResult()
        {
        }

        public PerformanceMonitorResult(TaskArgument argument, PerformanceLogFile logFile, DateTime logTime, int lineNumber)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
            this.LogTime = logTime;
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets 耗时
        /// </summary>
        public int Elapsed { get; set; }

        /// <summary>
        /// Gets or sets 请求路径
        /// </summary>
        public string RequestURI { get; set; }

        /// <summary>
        /// Gets or sets 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets 流长度
        /// </summary>
        public int StreamLength { get; set; }

        /// <summary>
        /// Gets or sets 消息
        /// </summary>
        public string Message { get; set; }
    }
}
