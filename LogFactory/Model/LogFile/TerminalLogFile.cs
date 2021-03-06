﻿using System;
using System.Collections.Generic;

using LogFactory.Model.Fixed;
using LogFactory.Model.Result;

namespace LogFactory.Model.LogFile
{
    /// <summary>
    /// 客户端和服务端日志文件
    /// </summary>
    public class TerminalLogFile : LogFileRelBase<TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalLogFile"/> class.
        /// </summary>
        public TerminalLogFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalLogFile"/> class.
        /// </summary>
        /// <param name="logFileTypes"></param>
        /// <param name="logLevel"></param>
        /// <param name="filePath"></param>
        /// <param name="createTime"></param>
        /// <param name="lastWriteTime"></param>
        /// <param name="relativePath"></param>
        public TerminalLogFile(LogFileTypes logFileTypes, LogLevels logLevel, string filePath, DateTime createTime, DateTime lastWriteTime, string relativePath)
        {
            this.LogFileType = logFileTypes;
            this.LogLevel = logLevel;
            this.FilePath = filePath;
            this.CreateTime = createTime;
            this.LastWriteTime = lastWriteTime;
            this.RelativePath = relativePath;
        }

        /// <summary>
        /// Gets or sets 未解析日志结果列表
        /// </summary>
        public virtual List<TerminalUnparsedResult> UnparsedResults { get; set; } = new List<TerminalUnparsedResult>();
    }
}
