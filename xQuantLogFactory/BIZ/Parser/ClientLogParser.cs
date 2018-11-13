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
    public class ClientLogParser : LogParserBase
    {

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        public override Regex LogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<Client>.*?)\s(?<Version>.*?)\s(?<IPAddress>.*?)\s(?<LogContent>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

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
                    while (!streamRreader.EndOfStream)
                    {
                        lineNumber++;
                        //获取日志行
                        string logLine = streamRreader.ReadLine();
                        Match match = this.LogRegex.Match(logLine);
                        if (match.Success)
                        {
                            //跳过未匹配到内容的日志行
                            if (!match.Groups["LogContent"].Success) continue;

                            DateTime logTime = DateTime.MinValue;
                            //跳过日志时间在任务时间范围外的日志行
                            if (match.Groups["LogTime"].Success &&
                                DateTime.TryParse(match.Groups["LogTime"].Value, out logTime) &&
                                match.Groups["Millisecond"].Success &&
                                double.TryParse(match.Groups["Millisecond"].Value, out double millisecond)
                                )
                                logTime = logTime.AddMilliseconds(millisecond);
                            else
                                continue;

                            if (!argument.CheckLogStartTime(logTime) ||
                                !argument.CheckLogFinishTime(logTime))
                                continue;

                            string logContent = match.Groups["LogContent"].Value;
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

                                if (match.Groups["LogLevel"].Success)
                                    result.LogLevel = match.Groups["LogLevel"].Value;

                                if (match.Groups["Version"].Success)
                                    result.Version = match.Groups["Version"].Value;

                                if (match.Groups["Client"].Success)
                                    result.Client = match.Groups["Client"].Value;

                                //客户端日志需要记录IP地址
                                if (match.Groups["IPAddress"].Success)
                                    result.IPAddress = match.Groups["IPAddress"].Value;

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
                    fileStream.Dispose();
                }
            });
        }

    }
}
