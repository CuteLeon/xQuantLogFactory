using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
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
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            List<LogFile> logFiles = new List<LogFile>();
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            Regex logRegex = new Regex(ConfigHelper.LogFileNameFormat, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (var (FullName, CreationTime, LastWriteTime) in directoryInfo.GetFiles("*.txt*", SearchOption.AllDirectories).Select(info => (info.FullName, info.CreationTime, info.LastWriteTime)))
            {
                string fileName = Path.GetFileName(FullName);
                //按格式筛选日志文件，以免查找到无用的文件
                if (!logRegex.IsMatch(fileName)) continue;

                if (!argument.CheckLogStartTime(CreationTime) &&
                    !argument.CheckLogStartTime(LastWriteTime))
                    continue;
                if (!argument.CheckLogFinishTime(CreationTime) &&
                    !argument.CheckLogFinishTime(LastWriteTime))
                    continue;

                logFiles.Add(new LogFile(
                    this.GetLogFileType(fileName),
                    FullName,
                    CreationTime,
                    LastWriteTime,
                    FullName.Remove(0, argument.LogDirectory.Length)
                    ));
            }

            return logFiles as IEnumerable<T>;
        }

        /// <summary>
        /// 获取日志文件类型
        /// </summary>
        /// <param name="fileName">日志文件名称</param>
        /// <returns></returns>
        private LogFileTypes GetLogFileType(string fileName)
        {
            if (fileName.StartsWith(ConfigHelper.ServerLogFileNamePrefix, StringComparison.OrdinalIgnoreCase))
                return LogFileTypes.Server;
            else if (fileName.StartsWith(ConfigHelper.ClientLogFileNamePrefix, StringComparison.OrdinalIgnoreCase))
                return LogFileTypes.Client;
            else if (fileName.StartsWith(ConfigHelper.MiddlewareLogFileNamePrefix, StringComparison.OrdinalIgnoreCase))
                return LogFileTypes.Middleware;

            return default;
        }

    }
}
