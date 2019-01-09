using System;

using xQuantLogFactory.Model.Fixed;

namespace xQuantLogFactory.Model.LogFile
{
    /// <summary>
    /// 文件基类
    /// </summary>
    public abstract class LogFileBase
    {
        #region 基础属性

        /// <summary>
        /// Gets or sets 日志文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件相对路径
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets 文件创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets 上次写入时间
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets or sets 日志文件类型
        /// </summary>
        public LogFileTypes LogFileType { get; set; }

        /// <summary>
        /// Gets or sets 日志等级
        /// </summary>
        public LogLevels LogLevel { get; set; }

        /// <summary>
        /// Gets 分析结果总耗时（单位：毫秒）
        /// </summary>
        public abstract double ElapsedMillisecond { get; }
        #endregion
    }
}
