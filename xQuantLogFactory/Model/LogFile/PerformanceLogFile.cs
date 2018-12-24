using System;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogFile"/> class.
        /// </summary>
        public PerformanceLogFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogFile"/> class.
        /// </summary>
        /// <param name="logFileTypes"></param>
        /// <param name="filePath"></param>
        /// <param name="createTime"></param>
        /// <param name="lastWriteTime"></param>
        /// <param name="relativePath"></param>
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
