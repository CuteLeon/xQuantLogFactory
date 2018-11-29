using System;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 日志解析结果抽象类
    /// </summary>
    public abstract class LogResultBase
    {
        /// <summary>
        /// Gets or sets 任务参数
        /// </summary>
        public TaskArgument TaskArgument { get; set; }

        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        public LogFile LogFile { get; set; }

        /// <summary>
        /// Gets or sets 日志文件中的行号
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets 日志写入时间
        /// </summary>
        public DateTime LogTime { get; set; }
    }
}
