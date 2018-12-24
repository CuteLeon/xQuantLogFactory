using System;
using System.Collections.Generic;

using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果
    /// </summary>
    public class TerminalAnalysisResult : AnalysisResultBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
    {
        public TerminalAnalysisResult()
        {
        }

        public TerminalAnalysisResult(TaskArgument argument, TerminalMonitorItem monitor, TerminalLogFile logFile)
        {
            this.LogFile = logFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;
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
        /// Gets 分析数据
        /// </summary>
        public Dictionary<string, object> AnalysisDatas { get; } = new Dictionary<string, object>();

        public override void BindMonitorResult(TerminalMonitorResult monitorResult)
        {
            base.BindMonitorResult(monitorResult);

            if (monitorResult == null)
            {
                this.Client = string.Empty;
                this.Version = string.Empty;
            }
            else
            {
                this.Client = monitorResult.Client;
                this.Version = monitorResult.Version;
            }
        }

        public override string ToString()
        {
            return $"【监视规则】={this.MonitorItem.PrefixName}\t【父级结果】={this.ParentAnalysisResult?.MonitorItem?.Name ?? "无"}\t【子结果数】={this.AnalysisResultRoots.Count}";
        }
    }
}
