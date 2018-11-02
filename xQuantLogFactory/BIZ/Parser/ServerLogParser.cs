using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 服务端日志解析器
    /// </summary>
    public class ServerLogParser : LogParserBase
    {
        /// <summary>
        /// 日志正则表达式
        /// </summary>
        /// <remarks>2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券</remarks>
        public override Regex LogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<Client>.*?)\s(?<Version>.*?)\s(?<LogContent>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public ServerLogParser() { }

        public ServerLogParser(ITrace trace) : base(trace) { }

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void Parse(TaskArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //遍历文件
            argument.LogFiles.Where(file => file.LogFileType == LogFileTypes.Server).AsParallel().ForAll(logFile =>
            {
                this.Trace?.WriteLine($"开始解析日志文件：(ID: {logFile.FileID}, Type: {logFile.LogFileType}) {logFile.FilePath}");

                using (StreamReader reader = new StreamReader(logFile.FilePath, Encoding.Default))
                {
                    int lineNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        lineNumber++;
                        //获取日志行
                        string logLine = reader.ReadLine();
                        Match match = this.LogRegex.Match(logLine);
                        if (match.Success)
                        {
                            //跳过未匹配到内容的日志行
                            if (!match.Groups["LogContent"].Success) continue;

                            string logContent = match.Groups["LogContent"].Value;
                            //匹配所有监视规则
                            foreach (MonitorItem monitor in argument.MonitorItems)
                            {
                                ResultTypes resultType = this.MatchMonitor(monitor, logContent);
                                if (resultType == ResultTypes.Unmatch)
                                {
                                    //未匹配到任何监视规则，尝试下一条规则
                                    continue;
                                }

                                MonitorResult result = new MonitorResult()
                                {
                                    ResultType = resultType,
                                    LineNumber = lineNumber,
                                    LogContent = logContent,
                                };

                                //反向关联日志监视结果
                                lock (argument)  //lock 任务而非 LockSeed 为了多任务并行考虑
                                {
                                    result.TaskArgument = argument;
                                    result.LogFile = logFile;
                                    result.MonitorItem = monitor;

                                    argument.MonitorResults.Add(result);
                                    logFile.MonitorResults.Add(result);
                                    monitor.MonitorResults.Add(result);
                                }

                                if (match.Groups["LogLevel"].Success)
                                    result.LogLevel = match.Groups["LogLevel"].Value;

                                //不能从此格式直接转换为日期时间类型，需要分开处理
                                if (match.Groups["LogTime"].Success &&
                                    DateTime.TryParse(match.Groups["LogTime"].Value, out DateTime logTime))
                                    result.LogTime = logTime;

                                if (match.Groups["Millisecond"].Success &&
                                    double.TryParse(match.Groups["Millisecond"].Value, out double millisecond))
                                    result.LogTime = result.LogTime.AddMilliseconds(millisecond);

                                if (match.Groups["Version"].Success)
                                    result.Version = match.Groups["Version"].Value;

                                if (match.Groups["Client"].Success)
                                    result.Client = match.Groups["Client"].Value;

                                this.Trace.WriteLine($"发现监视结果：\n\t文件ID= {logFile.FileID} 行号= {result.LineNumber} 等级= {result.LogLevel} 日志内容= {result.LogContent}");
                            }
                        }
                        else
                        {
                            //未识别的日志行
                        }
                    }

                    reader.Close();
                }
                this.Trace?.WriteLine($"当前日志文件(ID: {logFile.FileID})解析完成\n————————");
            });
        }

    }
}
