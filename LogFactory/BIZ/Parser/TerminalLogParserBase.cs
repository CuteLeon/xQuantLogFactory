using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using LogFactory.Model;
using LogFactory.Model.Extensions;
using LogFactory.Model.Fixed;
using LogFactory.Model.LogFile;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Parser
{
    /// <summary>
    /// 客户端和服务端日志解析抽象类
    /// </summary>
    public abstract class TerminalLogParserBase : LogParserBase<TerminalLogFile, TerminalMonitorResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalLogParserBase"/> class.
        /// </summary>
        public TerminalLogParserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalLogParserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public TerminalLogParserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN|SQL|MSG))\s(\s(?<ThreadID>\d*)\s)?(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Gets 日志详细内容正则表达式
        /// </summary>
        public abstract Regex ParticularRegex { get; }

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument"></param>
        public override void Parse(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 遍历文件
            this.GetFileFiltered(argument).AsParallel().ForAll(logFile =>
            {
                this.Tracer?.WriteLine($"<<<开始解析日志文件：{logFile.RelativePath}, Type: {logFile.LogFileType}, Level：{logFile.LogLevel}");

                // 检测日志文件解析耗时
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                FileStream fileStream = null;
                StreamReader streamRreader = null;
                try
                {
                    fileStream = new FileStream(logFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    streamRreader = new StreamReader(fileStream, Encoding.Default);

                    int lineNumber = 0, threadID = 0;
                    string logLine = string.Empty,
                        logLevel = string.Empty,
                        logContent = string.Empty;
                    Match generalMatch = null;
                    Match particularMatch = null;

                    // 缓存变量，减少树扫描次数，需要 .ToList()
                    List<TerminalMonitorItem> monitorItems = null;
                    lock (argument.MonitorContainerRoot)
                    {
                        monitorItems = argument.MonitorContainerRoot.GetTerminalMonitorItems().ToList();
                    }

                    while (!streamRreader.EndOfStream)
                    {
                        lineNumber++;

                        // 获取日志行
                        logLine = streamRreader.ReadLine();
                        lock (this.GeneralLogRegex)
                        {
                            generalMatch = this.GeneralLogRegex.Match(logLine);
                        }

                        if (generalMatch.Success)
                        {
                            // 跳过未匹配到内容的日志行
                            if (!generalMatch.Groups["LogContent"].Success)
                            {
                                continue;
                            }

                            DateTime logTime = DateTime.MinValue;

                            // 跳过日志时间在任务时间范围外的日志行
                            if (generalMatch.Groups["LogTime"].Success &&
                                DateTime.TryParse(generalMatch.Groups["LogTime"].Value, out logTime) &&
                                generalMatch.Groups["Millisecond"].Success &&
                                double.TryParse(generalMatch.Groups["Millisecond"].Value, out double millisecond))
                            {
                                logTime = logTime.AddMilliseconds(millisecond);
                            }
                            else
                            {
                                continue;
                            }

                            if (!argument.CheckLogTime(logTime))
                            {
                                continue;
                            }

                            logContent = this.PreprocessingLogContent(generalMatch.Groups["LogContent"].Value);
                            logLevel = generalMatch.Groups["LogLevel"].Success ? generalMatch.Groups["LogLevel"].Value : string.Empty;
                            threadID = generalMatch.Groups["ThreadID"].Success && int.TryParse(generalMatch.Groups["ThreadID"].Value, out threadID) ? threadID : -1;

                            // 精确匹配
                            if (this.ParticularRegex != null)
                            {
                                lock (this.ParticularRegex)
                                {
                                    particularMatch = this.ParticularRegex.Match(logContent);
                                }

                                if (particularMatch.Success && particularMatch.Groups["LogContent"].Success)
                                {
                                    logContent = particularMatch.Groups["LogContent"].Value;
                                }
                            }
                            else
                            {
                                particularMatch = null;
                            }

                            // 匹配所有监视规则
                            foreach (TerminalMonitorItem monitor in monitorItems)
                            {
                                GroupTypes groupType = monitor.MatchGroupLog(logContent);
                                if (groupType == GroupTypes.Unmatch)
                                {
                                    // 未匹配到任何监视规则，尝试下一条规则
                                    continue;
                                }

                                TerminalMonitorResult result = this.CreateMonitorResult(argument, logFile, monitor);
                                result.LogTime = logTime;
                                result.GroupType = groupType;
                                result.LineNumber = lineNumber;
                                result.LogContent = logContent;
                                result.LogLevel = logLevel;
                                result.ThreadID = threadID;

                                // 应用精准匹配数据
                                if (particularMatch?.Success ?? false)
                                {
                                    this.ApplyParticularMatch(result, particularMatch);
                                }

                                // this.Tracer.WriteLine($"发现监视结果：\n\t文件ID= {logFile.RelativePath} 行号= {result.LineNumber} 等级= {result.LogLevel} 日志内容= {result.LogContent}");
                            }
                        }
                        else
                        {
                            // 记录不为空的未识别的日志行
                            if (logLine.Length > 0)
                            {
                                logFile.UnparsedResults.Add(new TerminalUnparsedResult(argument, logFile, lineNumber, logLine));
                            }
                        }
                    }

                    // 停止日志文件解析计时器
                    stopWatch.Stop();
                    this.Tracer?.WriteLine($">>>日志文件解析完成：{logFile.RelativePath}\t结果数量：{logFile.MonitorResults.Count}\t耗时：{stopWatch.ElapsedMilliseconds.ToString("N0")} ms");
                }
                catch (Exception ex)
                {
                    // 停止日志文件解析计时器
                    stopWatch.Stop();
                    this.Tracer?.WriteLine($"——日志文件解析失败：{logFile.RelativePath}\t结果数量：{logFile.MonitorResults.Count}\t耗时：{stopWatch.ElapsedMilliseconds.ToString("N0")} ms\n\tException: {ex.Message}");
                }
                finally
                {
                    streamRreader?.Close();
                    streamRreader?.Dispose();

                    fileStream?.Close();
                    fileStream?.Dispose();
                }
            });

            // 监视结果解析完毕后按日志时间排序
            this.Tracer?.WriteLine(">>>————— 监视结果池排序 —————");
            argument.TerminalMonitorResults = argument.TerminalMonitorResults.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();
            this.Tracer?.WriteLine("<<< 排序完成");
        }

        /// <summary>
        /// 创建解析结果对象
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        protected TerminalMonitorResult CreateMonitorResult(TaskArgument argument, TerminalLogFile logFile, TerminalMonitorItem monitor)
        {
            TerminalMonitorResult monitorResult = new TerminalMonitorResult(argument, logFile, monitor);

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                argument.TerminalMonitorResults.Add(monitorResult);
                logFile.MonitorResults.Add(monitorResult);
                monitor.MonitorResults.Add(monitorResult);
            }

            return monitorResult;
        }
    }
}
