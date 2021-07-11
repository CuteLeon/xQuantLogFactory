using System;

using LogFactory.Model.Fixed;
using LogFactory.Model.LogFile;
using LogFactory.Model.Monitor;

namespace LogFactory.Model.Result
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
        /// Gets or sets 用户代码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// Gets or sets 请求发送时间
        /// </summary>
        public DateTime? RequestSendTime { get; set; }

        /// <summary>
        /// Gets or sets 请求接收时间
        /// </summary>
        public DateTime RequestReceiveTime { get; set; }

        /// <summary>
        /// Gets or sets 响应发送时间
        /// </summary>
        public DateTime ResponseSendTime { get; set; }

        /// <summary>
        /// Gets or sets 响应接收时间
        /// </summary>
        public DateTime? ResponseReceiveTime { get; set; }

        /// <summary>
        /// Gets or sets 耗时
        /// </summary>
        public double Elapsed { get; set; }

        /// <summary>
        /// Gets or sets 请求路径
        /// </summary>
        public string RequestURI { get; set; }

        /// <summary>
        /// Gets or sets 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets 请求流长度
        /// </summary>
        public double RequestStreamLength { get; set; }

        /// <summary>
        /// Gets or sets 响应流长度
        /// </summary>
        public double ResponseStreamLength { get; set; }

        /// <summary>
        /// Gets or sets 消息
        /// </summary>
        public string Message { get; set; }
        #endregion

        #region 业务

        /// <summary>
        /// Gets 请求延时
        /// </summary>
        public TimeSpan RequestDelay
        {
            get => this.GetTimeSpan(this.RequestSendTime, this.RequestReceiveTime);
        }

        /// <summary>
        /// Gets 响应延时
        /// </summary>
        public TimeSpan ResponseDelay
        {
            get => this.GetTimeSpan(this.ResponseSendTime, this.ResponseReceiveTime);
        }

        /// <summary>
        /// Gets 响应耗时
        /// </summary>
        public TimeSpan ResponseElapsed
        {
            get => this.GetTimeSpan(this.RequestReceiveTime, this.ResponseSendTime);
        }

        /// <summary>
        /// 获取时间差值
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="finishTime"></param>
        /// <returns></returns>
        private TimeSpan GetTimeSpan(DateTime? startTime, DateTime? finishTime)
        {
            if (startTime == null || finishTime == null)
            {
                return TimeSpan.Zero;
            }

            TimeSpan? span = finishTime - startTime;
            return span ?? TimeSpan.Zero;
        }

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
