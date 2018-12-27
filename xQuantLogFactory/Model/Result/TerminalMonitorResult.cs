using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 客户端和服务端解析结果
    /// </summary>
    public class TerminalMonitorResult : MonitorResultRelBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalMonitorResult"/> class.
        /// </summary>
        public TerminalMonitorResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalMonitorResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <param name="monitor"></param>
        public TerminalMonitorResult(TaskArgument argument, TerminalLogFile logFile, TerminalMonitorItem monitor)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
            this.MonitorItem = monitor;
        }

        #region 基础属性

        /// <summary>
        /// Gets or sets 客户端
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 程序版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets 日志级别
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// Gets or sets 日志内容
        /// </summary>
        public string LogContent { get; set; }
        #endregion

        #region 业务

        /// <summary>
        /// 检查解析是否与结果匹配
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        public override bool CheckMatch(TerminalMonitorResult targetResult)
        {
            if (targetResult == null)
            {
                return false;
            }

            if (this.GroupType == targetResult.GroupType)
            {
                return false;
            }

            bool matched =
                (string.IsNullOrEmpty(this.Version) || string.IsNullOrEmpty(targetResult.Version) || this.Version == targetResult.Version) &&
                (string.IsNullOrEmpty(this.Client) || string.IsNullOrEmpty(targetResult.Client) || this.Client == targetResult.Client) &&
                ((this.GroupType == GroupTypes.Start && this.LogTime <= targetResult.LogTime) || (this.GroupType == GroupTypes.Finish && this.LogTime >= targetResult.LogTime));

            return matched;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"【日志时间】={this.LogTime}，【监视规则】={this.MonitorItem?.Name}";
        }
        #endregion
    }
}
