﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Utils.Trace;

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
        public override Regex LogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<Client>.*?)\s(?<Version>.*?)\s(?<LogContent>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public ServerLogParser() { }

        public ServerLogParser(ITracer trace) : base(trace) { }

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

                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(logFile.FilePath, Encoding.Default);

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

                            //筛选日志行时间戳
                            if (logTime < argument.LogStartTime || logTime > argument.LogFinishTime)
                                continue;

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

                                //匹配到监视规则则新增一条监视结果记录，可能同一行日志会产生多条结果日志，因为同时与多条规则匹配
                                MonitorResult result = new MonitorResult()
                                {
                                    LogTime = logTime,
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

                    this.Trace?.WriteLine($"当前日志文件(ID: {logFile.FileID})解析完成\n————————");
                }
                catch (Exception ex)
                {
                    this.Trace?.WriteLine($"解析日志文件(ID: {logFile.FileID}) {logFile.FilePath} 失败：{ex.Message}\n————————");
                }
                finally
                {
                    reader?.Close();
                    reader?.Dispose();
                }
            });
        }

    }
}