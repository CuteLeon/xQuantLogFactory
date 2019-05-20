using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 客户端和服务端分析结果
    /// </summary>
    public class TerminalAnalysisResult : AnalysisResultRelBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalAnalysisResult"/> class.
        /// </summary>
        public TerminalAnalysisResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalAnalysisResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="monitor"></param>
        /// <param name="logFile"></param>
        public TerminalAnalysisResult(TaskArgument argument, TerminalMonitorItem monitor, TerminalLogFile logFile)
        {
            this.LogFile = logFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;
        }

        #region 基础属性

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 客户端版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets 线程ID
        /// </summary>
        public int ThreadID { get; set; }
        #endregion

        #region 业务

        /// <summary>
        /// 绑定父分析结果节点
        /// </summary>
        /// <param name="monitorResult"></param>
        public override void BindMonitorResult(TerminalMonitorResult monitorResult)
        {
            base.BindMonitorResult(monitorResult);

            if (monitorResult == null)
            {
                this.Client = string.Empty;
                this.Version = string.Empty;
                this.ThreadID = -1;
            }
            else
            {
                this.Client = monitorResult.Client;
                this.Version = monitorResult.Version;
                this.ThreadID = monitorResult.ThreadID;
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"【监视规则】={this.MonitorItem.PrefixName}\t【父级结果】={this.ParentAnalysisResult?.MonitorItem?.Name ?? "无"}\t【子结果数】={this.AnalysisResultRoots.Count}";
        }
        #endregion
    }
}
