using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Utils.Extensions;

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
            Regex logRegex = argument.GetLogLevelRegex();

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

                /* 复制或移动日志文件将会改变日志文件的创建时间、
                 * 手动摘取日志文件将会改变日志文件的最后写入时间，
                 * 可能造成日志文件按任务时间筛选出错，因此不使用任务时间筛选日志文件，而是处理目录内所有日志文件；
                 */

                // if (argument.CheckLogFileTime(creationTime, lastWriteTime))
                {
                    LogFileTypes fileType = this.GetLogFileType(fileName);
                    LogLevels logLevel = this.GetLogLevel(fileName);
                    switch (logLevel)
                    {
                        case LogLevels.Debug:
                        case LogLevels.Info:
                        case LogLevels.Trace:
                        case LogLevels.Warn:
                        case LogLevels.Error:
                            {
                                argument.TerminalLogFiles.Add(new TerminalLogFile(
                                    fileType,
                                    logLevel,
                                    fullName,
                                    creationTime,
                                    lastWriteTime,
                                    fullName.Remove(0, argument.LogDirectory.Length)));
                                break;
                            }

                        case LogLevels.Perf:
                            {
                                argument.PerformanceLogFiles.Add(new PerformanceLogFile(
                                    fileType,
                                    logLevel,
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
            if (fileName.IndexOf($"{FixedDatas.ClientLogFileNamePrefix}Log_", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return LogFileTypes.Client;
            }
            else if (fileName.StartsWith(FixedDatas.ServerLogFileNamePrefix, StringComparison.OrdinalIgnoreCase))
            {
                return LogFileTypes.Server;
            }

            throw new Exception($"未知的日志文件类型：{fileName}");
        }

        /// <summary>
        /// 获取日志等级
        /// </summary>
        /// <param name="fileName">日志文件名称</param>
        /// <returns></returns>
        public LogLevels GetLogLevel(string fileName)
        {
            foreach (LogLevels level in Enum.GetValues(typeof(LogLevels)))
            {
                if (fileName.IndexOf($"_{level.GetAmbientValue()}.", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return level;
                }
            }

            throw new Exception($"未知的日志等级：{fileName}");
        }
    }
}
