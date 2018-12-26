using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// Performance日志解析器
    /// </summary>
    public class PerformanceLogParser : LogParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogParser"/> class.
        /// </summary>
        public PerformanceLogParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogParser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public PerformanceLogParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<IPAddress>\d{1,3}(\.\d{1,3}){3})\s(?<UserCode>.*?)\s(?<StartTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Elapsed>\d*?)\s(?<RequestURI>\/.*?)\s(?<MethodName>.*?)\s(?<StreamLength>\d*?)\s(?<Message>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void Parse(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 遍历文件
            argument.PerformanceLogFiles.AsParallel().ForAll(logFile =>
            {
                this.Tracer?.WriteLine($"<<<开始解析日志文件：{logFile.RelativePath}, Type: {logFile.LogFileType}");

                // 检测日志文件解析耗时
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                FileStream fileStream = null;
                StreamReader streamRreader = null;
                try
                {
                    fileStream = new FileStream(logFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    streamRreader = new StreamReader(fileStream, Encoding.Default);

                    // 临时变量放于循环外，防止内存爆炸
                    Match match = null;
                    string logLine = string.Empty,
                              message = string.Empty,
                              methodName = string.Empty;
                    int lineNumber = 0;
                    DateTime logTime = DateTime.MinValue;

                    // 缓存变量，减少树扫描次数，需要 .ToList()
                    List<PerformanceMonitorItem> monitorItems = null;
                    lock (argument.MonitorContainerRoot)
                    {
                        monitorItems = argument.MonitorContainerRoot.GetPerformanceMonitorItems().ToList();
                    }

                    while (!streamRreader.EndOfStream)
                    {
                        lineNumber++;

                        // 获取日志行
                        logLine = streamRreader.ReadLine();
                        lock (this.GeneralLogRegex)
                        {
                            match = this.GeneralLogRegex.Match(logLine);
                        }

                        if (match.Success)
                        {
                            // 跳过日志时间在任务时间范围外的日志行
                            if (!match.Groups["LogTime"].Success ||
                                !DateTime.TryParse(match.Groups["LogTime"].Value, out logTime))
                            {
                                continue;
                            }

                            if (!argument.CheckLogTime(logTime))
                            {
                                continue;
                            }

                            // 匹配所有监视规则
                            foreach (PerformanceMonitorItem monitor in monitorItems)
                            {
                                message = match.Groups["Message"].Value;
                                methodName = match.Groups["MethodName"].Value;
                                GroupTypes groupType = monitor.MatchGroupLog(methodName);
                                PerformanceTypes performanceType = monitor.MatchPerformanceType(message);

                                if (groupType == GroupTypes.Unmatch)
                                {
                                    // 未匹配到任何监视规则，尝试下一条规则
                                    continue;
                                }

                                PerformanceMonitorResult result = this.CreateMonitorResult(argument, logFile, monitor);
                                result.LogTime = logTime;
                                result.Message = message;
                                result.GroupType = groupType;
                                result.LineNumber = lineNumber;
                                result.MethodName = methodName;
                                result.PerformanceType = performanceType;

                                this.ApplyRegexMatch(match, result);

                                // this.Tracer.WriteLine($"发现监视结果：\n\t文件ID= {logFile.RelativePath} 行号= {result.LineNumber} IP地址= {result.IPAddress} 方法名称= {result.MethodName}");
                            }
                        }
                        else
                        {
                            // 未识别的日志行
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
            argument.PerformanceMonitorResults = argument.PerformanceMonitorResults.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();
            this.Tracer?.WriteLine("<<< 排序完成");
        }

        /// <summary>
        /// 将正则匹配结果应用到监视结果
        /// </summary>
        /// <param name="match"></param>
        /// <param name="result"></param>
        protected void ApplyRegexMatch(Match match, in PerformanceMonitorResult result)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (match.Groups["IPAddress"].Success)
            {
                result.IPAddress = match.Groups["IPAddress"].Value;
            }

            if (match.Groups["UserCode"].Success)
            {
                result.UserCode = match.Groups["UserCode"].Value;
            }

            if (match.Groups["StartTime"].Success && DateTime.TryParse(match.Groups["StartTime"].Value, out DateTime startTime))
            {
                result.StartTime = startTime;
            }

            if (match.Groups["Elapsed"].Success && int.TryParse(match.Groups["Elapsed"].Value, out int elapsed))
            {
                result.Elapsed = elapsed;
            }

            if (match.Groups["StreamLength"].Success && int.TryParse(match.Groups["StreamLength"].Value, out int streamLength))
            {
                result.StreamLength = streamLength;
            }

            if (match.Groups["RequestURI"].Success)
            {
                result.RequestURI = match.Groups["RequestURI"].Value;
            }
        }

        /// <summary>
        /// 创建解析结果对象
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        protected PerformanceMonitorResult CreateMonitorResult(TaskArgument argument, PerformanceLogFile logFile, PerformanceMonitorItem monitor)
        {
            PerformanceMonitorResult monitorResult = new PerformanceMonitorResult(argument, logFile, monitor);

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                argument.PerformanceMonitorResults.Add(monitorResult);
                logFile.MonitorResults.Add(monitorResult);
                monitor.MonitorResults.Add(monitorResult);
            }

            return monitorResult;
        }
    }
}
