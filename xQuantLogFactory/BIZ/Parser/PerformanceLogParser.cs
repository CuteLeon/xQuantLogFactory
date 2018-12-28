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
                              methodName = string.Empty,
                              ipAddress = string.Empty,
                              userCode = string.Empty,
                              requestURI = string.Empty;
                    int lineNumber = 0,
                         elapsed = 0,
                         streamLength = 0;
                    DateTime logTime = DateTime.MinValue,
                                    startTime = DateTime.MinValue;
                    PerformanceTypes performanceType = PerformanceTypes.Unknown;

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

                            methodName = match.Groups["MethodName"].Value;

                            // 检查方法名称是否在黑名单中
                            if (this.CheckLogInBlackList(methodName))
                            {
                                continue;
                            }

                            ipAddress = match.Groups["IPAddress"].Value;
                            userCode = match.Groups["UserCode"].Value;
                            startTime = DateTime.TryParse(match.Groups["StartTime"].Value, out DateTime startTimeValue) ? startTimeValue : DateTime.MinValue;
                            elapsed = int.TryParse(match.Groups["Elapsed"].Value, out int elapsedValue) ? elapsedValue : 0;
                            streamLength = int.TryParse(match.Groups["StreamLength"].Value, out int streamLengthValue) ? streamLengthValue : 0;
                            requestURI = match.Groups["RequestURI"].Value;
                            message = match.Groups["Message"].Value;
                            performanceType = this.MatchPerformanceType(message);

                            // 记录所有解析结果
                            PerformanceMonitorResult parseResult = this.CreateParseResult(argument, logFile);
                            parseResult.LogTime = logTime;
                            parseResult.Message = message;
                            parseResult.GroupType = GroupTypes.Unmatch;
                            parseResult.LineNumber = lineNumber;
                            parseResult.MethodName = methodName;
                            parseResult.PerformanceType = performanceType;
                            parseResult.Elapsed = elapsed;
                            parseResult.IPAddress = ipAddress;
                            parseResult.UserCode = userCode;
                            parseResult.StartTime = startTime;
                            parseResult.StreamLength = streamLength;
                            parseResult.RequestURI = requestURI;

                            // 匹配所有监视规则
                            foreach (PerformanceMonitorItem monitor in monitorItems)
                            {
                                GroupTypes groupType = monitor.MatchGroupLog(methodName);

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
                                result.Elapsed = elapsed;
                                result.IPAddress = ipAddress;
                                result.UserCode = userCode;
                                result.StartTime = startTime;
                                result.StreamLength = streamLength;
                                result.RequestURI = requestURI;

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
                    this.Tracer?.WriteLine($">>>日志文件解析完成：{logFile.RelativePath}\t解析结果：{logFile.PerformanceParseResults.Count}\t监视结果：{logFile.MonitorResults.Count}\t耗时：{stopWatch.ElapsedMilliseconds.ToString("N0")} ms");
                }
                catch (Exception ex)
                {
                    // 停止日志文件解析计时器
                    stopWatch.Stop();
                    this.Tracer?.WriteLine($"——日志文件解析失败：{logFile.RelativePath}\t解析结果：{logFile.PerformanceParseResults.Count}\t监视结果：{logFile.MonitorResults.Count}\t耗时：{stopWatch.ElapsedMilliseconds.ToString("N0")} ms\n\tException: {ex.Message}");
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
        /// 创建监视结果对象
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

        /// <summary>
        /// 创建解析结果对象
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <returns></returns>
        protected PerformanceMonitorResult CreateParseResult(TaskArgument argument, PerformanceLogFile logFile)
        {
            PerformanceMonitorResult monitorResult = new PerformanceMonitorResult(argument, logFile);

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                argument.PerformanceParseResults.Add(monitorResult);
                logFile.PerformanceParseResults.Add(monitorResult);
            }

            return monitorResult;
        }

        /// <summary>
        /// 匹配日志事件类型
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual PerformanceTypes MatchPerformanceType(string message)
        {
            if (message.Equals(FixedDatas.PERFORMANCE_START_MESSAGE, StringComparison.Ordinal))
            {
                return PerformanceTypes.Start;
            }
            else if (message.Equals(FixedDatas.PERFORMANCE_FINISH_MESSAGE, StringComparison.Ordinal))
            {
                return PerformanceTypes.Finish;
            }
            else
            {
                return PerformanceTypes.Unknown;
            }
        }

        /// <summary>
        /// 检查日志是否在黑名单中而放弃
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected virtual bool CheckLogInBlackList(string log)
            => FixedDatas.MethodNameBlackList.IndexOf(log) > -1;
    }
}
