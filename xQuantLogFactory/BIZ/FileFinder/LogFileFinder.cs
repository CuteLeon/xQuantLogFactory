using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

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
        public IEnumerable<T> GetFiles<T>(string directory, TaskArgument argument) where T : class
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));
            if (!Directory.Exists(argument.BaseDirectory))
                throw new DirectoryNotFoundException(argument.BaseDirectory);

            List<LogFile> logFiles = new List<LogFile>();
            DirectoryInfo directoryInfo = new DirectoryInfo(argument.BaseDirectory);
            Regex logRegex = new Regex(ConfigHelper.LogFileNameFormat, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (var (FullName, CreationTime, LastWriteTime) in directoryInfo.GetFiles("*.txt*").Select(info => (info.FullName, info.CreationTime, info.LastWriteTime)))
            {
                string fileName = Path.GetFileName(FullName);
                //按格式筛选日志文件，以免查找到无用的文件
                if (!logRegex.IsMatch(fileName)) continue;

                if ((CreationTime > argument.LogStartTime && CreationTime < argument.LogFinishTime) ||
                    (LastWriteTime > argument.LogStartTime && LastWriteTime < argument.LogFinishTime)
                    )
                {
                    logFiles.Add(new LogFile()
                    {
                        LogFileType = fileName.StartsWith(ConfigHelper.ServerLogFileNamerefix) ? LogFileTypes.Server : LogFileTypes.Client,
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
