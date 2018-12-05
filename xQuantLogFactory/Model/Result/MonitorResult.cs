using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 监视规则解析结果
    /// </summary>
    public class MonitorResult : LogResultBase
    {
        public MonitorResult()
        {
        }

        public MonitorResult(TaskArgument argument, LogFile logFile, MonitorItem monitor)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
            this.MonitorItem = monitor;
        }

        /// <summary>
        /// Gets or sets 监控项目
        /// </summary>
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// Gets or sets 监视结果匹配模式
        /// </summary>
        public GroupTypes GroupType { get; set; }

        /// <summary>
        /// Gets or sets 客户端名称
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets 程序版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets iP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets 日志级别
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// Gets or sets 日志内容
        /// </summary>
        public string LogContent { get; set; }

        /// <summary>
        /// 检查解析是否与结果匹配
        /// </summary>
        /// <param name="targetResult"></param>
        /// <returns></returns>
        public bool CheckMatch(MonitorResult targetResult)
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

        public override string ToString()
        {
            return $"【日志时间】={this.LogTime}，【监视规则】={this.MonitorItem?.Name}";
        }
    }
}
