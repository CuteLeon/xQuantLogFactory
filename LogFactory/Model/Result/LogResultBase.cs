using System;

using LogFactory.Model.LogFile;

namespace LogFactory.Model.Result
{
    /// <summary>
    /// 日志解析结果抽象类
    /// </summary>
    /// <typeparam name="TLogFile"></typeparam>
    public abstract class LogResultBase<TLogFile>
        where TLogFile : LogFileBase
    {
        /// <summary>
        /// Gets or sets 任务参数
        /// </summary>
        public TaskArgument TaskArgument { get; set; }

        /// <summary>
        /// Gets or sets 日志文件中的行号
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        public TLogFile LogFile { get; set; }

        /// <summary>
        /// Gets or sets 日志写入时间
        /// </summary>
        public DateTime LogTime { get; set; }
    }
}
