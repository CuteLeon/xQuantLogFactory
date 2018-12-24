using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 日志文件查找器
    /// </summary>
    public class LogFileFinder : TaskFileFinderBase
    {
        /// <summary>
        /// 查找符合日志分析参数的待分析日志文件清单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory">文件存放目录</param>
        /// <param name="argument">任务参数</param>
        /// <returns>返回符合日志分析参数的待分析日志文件清单</returns>
        public override T GetTaskObject<T>(string directory, TaskArgument argument)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(nameof(directory));
            }

            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            Regex logRegex = new Regex(ConfigHelper.LogFileNameFormat, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

            foreach (var (fullName, creationTime, lastWriteTime) in directoryInfo
                .GetFiles("*.txt*", SearchOption.AllDirectories)
                .Select(info => (info.FullName, info.CreationTime, info.LastWriteTime)))
            {
                string fileName = Path.GetFileName(fullName);

                // 按格式筛选日志文件，以免查找到无用的文件
                if (!logRegex.IsMatch(fileName))
                {
                    continue;
                }

                if (argument.CheckLogFileTime(creationTime, lastWriteTime))
                {
                    LogFileTypes fileType = this.GetLogFileType(fileName);
                    switch (fileType)
                    {
                        case LogFileTypes.Client:
                        case LogFileTypes.Server:
                            {
                                argument.TerminalLogFiles.Add(new TerminalLogFile(
                                    fileType,
                                    fullName,
                                    creationTime,
                                    lastWriteTime,
                                    fullName.Remove(0, argument.LogDirectory.Length)));
                                break;
                            }

                        case LogFileTypes.Performance:
                            {
                                argument.PerformanceLogFiles.Add(new PerformanceLogFile(
                                    fileType,
                                    fullName,
                                    creationTime,
                                    lastWriteTime,
                                    fullName.Remove(0, argument.LogDirectory.Length)));
                                break;
                            }

                        default:
                            break;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取日志文件类型
        /// </summary>
        /// <param name="fileName">日志文件名称</param>
        /// <returns></returns>
        public LogFileTypes GetLogFileType(string fileName)
        {
            if (fileName.IndexOf($"{ConfigHelper.ClientLogFileNamePrefix}Log_", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return LogFileTypes.Client;
            }
            else if (fileName.StartsWith(ConfigHelper.ServerLogFileNamePrefix, StringComparison.OrdinalIgnoreCase))
            {
                return LogFileTypes.Server;
            }
            else if (fileName.StartsWith(ConfigHelper.PerformanceLogFileNamePrefix, StringComparison.OrdinalIgnoreCase))
            {
                return LogFileTypes.Performance;
            }

            return default;
        }
    }
}
