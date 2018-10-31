using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 日志文件查找器
    /// </summary>
    public class LogFileFinder : ITaskFileFinder
    {

        /// <summary>
        /// 查找符合日志分析参数的待分析日志文件清单
        /// </summary>
        /// <param name="directory">文件存放目录</param>
        /// <param name="param">任务参数</param>
        /// <returns>返回符合日志分析参数的待分析日志文件清单</returns>
        public IEnumerable<T> GetFiles<T>(string directory, TaskArgument argumant) where T : class
        {
            if (argumant == null)
                throw new ArgumentNullException(nameof(argumant));
            if (!Directory.Exists(argumant.BaseDirectory))
                throw new DirectoryNotFoundException(argumant.BaseDirectory);

            List<LogFile> logFiles = new List<LogFile>();
            DirectoryInfo directoryInfo = new DirectoryInfo(argumant.BaseDirectory);

            foreach (var (FullName, CreationTime, LastWriteTime) in directoryInfo.GetFiles("*.txt*").Select(info => (info.FullName, info.CreationTime, info.LastWriteTime)))
            {
                if ((CreationTime > argumant.LogStartTime && CreationTime < argumant.LogFinishTime) ||
                    (LastWriteTime > argumant.LogStartTime && LastWriteTime < argumant.LogFinishTime)
                    )
                {
                    logFiles.Add(new LogFile()
                    {
                        FilePath = FullName,
                        CreateTime = CreationTime,
                        LastWriteTime = LastWriteTime,
                    });
                }
            }

            return logFiles as IEnumerable<T>;
        }

    }
}
