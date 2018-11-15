using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 客户端日志解析器
    /// </summary>
    public class ClientLogParser : CSLogParserBase
    {
        /// <summary>
        /// 日志详细内容正则表达式
        /// </summary>
        public override Regex ParticularRegex { get; } = new Regex(
            @"^(?<Client>.*?)\s(?<Version>.*?(\.[^\s]*){3})\s(?<IPAddress>\d{1,3}(\.\d{1,3}){3})\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public ClientLogParser() { }

        public ClientLogParser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void Parse(TaskArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //遍历文件
            argument.LogFiles.Where(file => file.LogFileType == LogFileTypes.Client).AsParallel().ForAll(logFile =>
            {
                this.Trace?.WriteLine($"开始解析日志文件：(ID: {logFile.FileID}, Type: {logFile.LogFileType}) {logFile.FilePath}");

                FileStream fileStream = null;
                StreamReader streamRreader = null;
                try
                {
                    fileStream = new FileStream(logFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    streamRreader = new StreamReader(fileStream, Encoding.Default);

                    int lineNumber = 0;
                    string logLine = string.Empty,
                        logLevel = string.Empty,
                        client = string.Empty,
                        version = string.Empty,
                        ipAddress = string.Empty,
                        logContent = string.Empty;
                    Match generalMatch = null;
                    Match particularMatch = null;

                    while (!streamRreader.EndOfStream)
                    {
                        lineNumber++;
                        //获取日志行
                        logLine = streamRreader.ReadLine();
                        generalMatch = this.GeneralLogRegex.Match(logLine);
                        if (generalMatch.Success)
                        {
                            //跳过未匹配到内容的日志行
                            if (!generalMatch.Groups["LogContent"].Success) continue;

                            DateTime logTime = DateTime.MinValue;
                            //跳过日志时间在任务时间范围外的日志行
                            if (generalMatch.Groups["LogTime"].Success &&
                                DateTime.TryParse(generalMatch.Groups["LogTime"].Value, out logTime) &&
                                generalMatch.Groups["Millisecond"].Success &&
                                double.TryParse(generalMatch.Groups["Millisecond"].Value, out double millisecond)
                                )
                                logTime = logTime.AddMilliseconds(millisecond);
                            else
                                continue;

                            if (!argument.CheckLogStartTime(logTime) ||
                                !argument.CheckLogFinishTime(logTime))
                                continue;

                            logContent = generalMatch.Groups["LogContent"].Value;
                            logLevel = generalMatch.Groups["LogLevel"].Success ? generalMatch.Groups["LogLevel"].Value : string.Empty;

                            //精确匹配
                            particularMatch = this.ParticularRegex.Match(logContent);
                            if (particularMatch.Success)
                            {
                                version = particularMatch.Groups["Version"].Success ? particularMatch.Groups["Version"].Value : string.Empty;
                                client = particularMatch.Groups["Client"].Success ? particularMatch.Groups["Client"].Value : string.Empty;
                                ipAddress = particularMatch.Groups["IPAddress"].Success ? particularMatch.Groups["IPAddress"].Value : string.Empty;
                                logContent = particularMatch.Groups["LogContent"].Success ? particularMatch.Groups["LogContent"].Value : string.Empty;
                            }

                            //匹配所有监视规则
                            foreach (MonitorItem monitor in argument.MonitorItems)
                            {
                                GroupTypes groupType = this.MatchMonitor(monitor, logContent);
                                if (groupType == GroupTypes.Unmatch)
                                {
                                    //未匹配到任何监视规则，尝试下一条规则
                                    continue;
                                }

                                MonitorResult result = this.CreateMonitorResult(argument, logFile, monitor);
                                result.LogTime = logTime;
                                result.GroupType = groupType;
                                result.LineNumber = lineNumber;
                                result.LogContent = logContent;
                                result.LogLevel = logLevel;
                                result.Client = client;
                                result.Version = version;
                                result.IPAddress = ipAddress;

                                this.Trace.WriteLine($"发现监视结果：\n\t文件ID= {logFile.FileID} 行号= {result.LineNumber} 等级= {result.LogLevel} 日志内容= {result.LogContent}");
                            }
                        }
                        else
                        {
                            //未识别的日志行
                        }
                    }

                    this.Trace?.WriteLine($"当前日志文件(ID: {logFile.FileID})解析完成\n————————");
                }
                catch (Exception ex)
                {
                    this.Trace?.WriteLine($"解析日志文件(ID: {logFile.FileID}) {logFile.FilePath} 失败：{ex.Message}\n————————");
                }
                finally
                {
                    streamRreader?.Close();
                    streamRreader?.Dispose();

                    fileStream?.Close();
                    fileStream?.Dispose();
                }
            });
        }

    }
}
