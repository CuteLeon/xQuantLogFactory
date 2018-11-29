﻿using System;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 组匹配类型
    /// </summary>
    public enum GroupTypes
    {
        /// <summary>
        /// 未匹配
        /// </summary>
        Unmatch = 0,

        /// <summary>
        /// 开始匹配
        /// </summary>
        Start = 1,

        /// <summary>
        /// 结束匹配
        /// </summary>
        Finish = 2,
    }

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
