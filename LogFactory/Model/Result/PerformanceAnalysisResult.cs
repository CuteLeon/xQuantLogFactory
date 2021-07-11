using LogFactory.Model.LogFile;
using LogFactory.Model.Monitor;

namespace LogFactory.Model.Result
{
    /// <summary>
    /// Performance分析结果
    /// </summary>
    public class PerformanceAnalysisResult : AnalysisResultRelBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceAnalysisResult"/> class.
        /// </summary>
        public PerformanceAnalysisResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceAnalysisResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="monitor"></param>
        /// <param name="logFile"></param>
        public PerformanceAnalysisResult(TaskArgument argument, PerformanceMonitorItem monitor, PerformanceLogFile logFile)
        {
            this.LogFile = logFile;
            this.MonitorItem = monitor;
            this.TaskArgument = argument;
        }

        #region 基础属性

        /// <summary>
        /// Gets or sets 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets iP地址
        /// </summary>
        public string IPAddress { get; set; }
        #endregion

        /// <summary>
        /// 绑定父分析结果节点
        /// </summary>
        /// <param name="monitorResult"></param>
        public override void BindMonitorResult(PerformanceMonitorResult monitorResult)
        {
            base.BindMonitorResult(monitorResult);

            if (monitorResult == null)
            {
                this.IPAddress = string.Empty;
                this.UserCode = string.Empty;
            }
            else
            {
                this.IPAddress = monitorResult.IPAddress;
                this.UserCode = monitorResult.UserCode;
            }
        }
    }
}
