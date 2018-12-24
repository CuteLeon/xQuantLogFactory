using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.LogFile
{
    /// <summary>
    /// 日志文件
    /// </summary>
    public class TerminalLogFile : LogFileBase<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>
    {
        public TerminalLogFile()
        {
        }

        public TerminalLogFile(LogFileTypes logFileTypes, string filePath, DateTime createTime, DateTime lastWriteTime, string relativePath)
        {
            this.LogFileType = logFileTypes;
            this.FilePath = filePath;
            this.CreateTime = createTime;
            this.LastWriteTime = lastWriteTime;
            this.RelativePath = relativePath;
        }
    }
}
