using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.LogFile
{
    /// <summary>
    /// Performance 日志文件
    /// </summary>
    public class PerformanceLogFile : LogFileBase<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>
    {
        public PerformanceLogFile(LogFileTypes logFileTypes, string filePath, DateTime createTime, DateTime lastWriteTime, string relativePath)
        {
            this.LogFileType = logFileTypes;
            this.FilePath = filePath;
            this.CreateTime = createTime;
            this.LastWriteTime = lastWriteTime;
            this.RelativePath = relativePath;
        }
    }
}
