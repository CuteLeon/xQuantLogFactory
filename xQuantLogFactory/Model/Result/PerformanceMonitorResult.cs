using System;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// Performance解析结果
    /// </summary>
    public class PerformanceMonitorResult : MonitorResultRelBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorResult"/> class.
        /// </summary>
        public PerformanceMonitorResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        public PerformanceMonitorResult(TaskArgument argument, PerformanceLogFile logFile)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <param name="monitor"></param>
        public PerformanceMonitorResult(TaskArgument argument, PerformanceLogFile logFile, PerformanceMonitorItem monitor)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
            this.MonitorItem = monitor;
        }

        #region 基础属性

        /// <summary>
        /// Gets or sets performance日志类型
        /// </summary>
        public PerformanceTypes PerformanceType { get; set; }

        /// <summary>
        /// Gets or sets 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

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
        #endregion

        #region 业务

        /// <summary>
        /// 检查两个监视结果是否匹配
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        public override bool CheckMatch(PerformanceMonitorResult targetResult)
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
                (string.IsNullOrEmpty(this.IPAddress) || string.IsNullOrEmpty(targetResult.IPAddress) || this.IPAddress == targetResult.IPAddress) &&
                (string.IsNullOrEmpty(this.UserCode) || string.IsNullOrEmpty(targetResult.UserCode) || this.UserCode == targetResult.UserCode) &&
                ((this.GroupType == GroupTypes.Start && this.LogTime <= targetResult.LogTime) || (this.GroupType == GroupTypes.Finish && this.LogTime >= targetResult.LogTime));

            return matched;
        }
        #endregion
    }
}
