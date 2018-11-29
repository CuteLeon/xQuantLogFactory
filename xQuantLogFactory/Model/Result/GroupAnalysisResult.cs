using System.Collections.Generic;

using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果
    /// </summary>
    public class GroupAnalysisResult
    {
        public GroupAnalysisResult()
        {
        }

        public GroupAnalysisResult(TaskArgument argument, LogFile logFile, MonitorItem monitor, MonitorResult monitorResult)
        {
            this.LogFile = logFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;

            if (monitorResult != null)
            {
                this.Client = monitorResult.Client;
                this.Version = monitorResult.Version;
                this.LineNumber = monitorResult.LineNumber;
            }
        }

        /// <summary>
        /// Gets or sets 任务参数
        /// </summary>
        public TaskArgument TaskArgument { get; set; }

        /// <summary>
        /// Gets or sets 行号
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 客户端版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        public LogFile LogFile { get; set; }

        /// <summary>
        /// Gets or sets 监控项目
        /// </summary>
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 匹配开始结果
        /// </summary>
        public MonitorResult StartMonitorResult { get; set; }

        /// <summary>
        /// Gets or sets 匹配结束结果
        /// </summary>
        public MonitorResult FinishMonitorResult { get; set; }

        /// <summary>
        /// Gets or sets 结果耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets 分析数据
        /// </summary>
        public Dictionary<string, object> AnalysisDatas { get; } = new Dictionary<string, object>();
    }
}
