using System.Collections.Generic;

using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果
    /// </summary>
    public class GroupAnalysisResult : LogResultBase, IAnalysisResult
    {
        public GroupAnalysisResult()
        {
        }

        public GroupAnalysisResult(TaskArgument argument, MonitorItem monitor, MonitorResult monitorResult)
        {
            this.LogFile = monitorResult.LogFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;

            if (monitorResult != null)
            {
                this.Client = monitorResult.Client;
                this.Version = monitorResult.Version;
                this.LineNumber = monitorResult.LineNumber;
                this.LogTime = monitorResult.LogTime;
            }
        }

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 客户端版本
        /// </summary>
        public string Version { get; set; }

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
        /// Gets or sets 父分析结果
        /// </summary>
        public GroupAnalysisResult ParentAnalysisResult { get; set; }

        /// <summary>
        /// Gets or sets 子分析结果
        /// </summary>
        public List<GroupAnalysisResult> AnalysisResultRoots { get; set; } = new List<GroupAnalysisResult>();

        /// <summary>
        /// Gets or sets 结果耗时（单位：毫秒）
        /// </summary>
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets 分析数据
        /// </summary>
        public Dictionary<string, object> AnalysisDatas { get; } = new Dictionary<string, object>();

        public override string ToString()
        {
            return $"【监视规则】={this.MonitorItem.PrefixName}\t【父级结果】={this.ParentAnalysisResult?.MonitorItem?.Name ?? "无"}\t【子结果数】={this.AnalysisResultRoots.Count}";
        }
    }
}
