using System;
using System.Collections.Generic;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.LogFile
{
    /// <summary>
    /// 客户端和服务端日志文件
    /// </summary>
    public class TerminalLogFile : LogFileBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
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
        /// <param name="filePath"></param>
        /// <param name="createTime"></param>
        /// <param name="lastWriteTime"></param>
        /// <param name="relativePath"></param>
        public TerminalLogFile(LogFileTypes logFileTypes, string filePath, DateTime createTime, DateTime lastWriteTime, string relativePath)
        {
            this.LogFileType = logFileTypes;
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
