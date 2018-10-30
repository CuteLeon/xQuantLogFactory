namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视规则解析结果
    /// </summary>
    public class MonitorItemResult
    {
        /// <summary>
        /// 日志文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 日志写入时间
        /// </summary>
        public string LogTime { get; set; }

        /// <summary>
        /// 监控项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 匹配模式
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// 日志文件中的行号
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// 程序版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string LogContent { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", this.FileName, this.Pattern, this.LogTime, this.LogContent);
        }
    }
}
