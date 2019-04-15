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
using xQuantLogFactory.Utils.Extensions;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// Performance日志解析器基类
    /// </summary>
    public abstract class PerformanceLogParserBase : LogParserBase<PerformanceLogFile, PerformanceMonitorResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogParserBase"/> class.
        /// </summary>
        public PerformanceLogParserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogParserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public PerformanceLogParserBase(ITracer tracer)
            : base(tracer)
        {
        }

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

                    // 临时变量放于循环外，防止内存爆炸
                    Match match = null;
                    string logLine = string.Empty,
                              requestURI = string.Empty,
                              methodName = string.Empty;
                    double elapsed = 0.0,
                         requestStreamLength = 0.0,
                         responseStreamLength = 0.0;
                    int lineNumber = 0;

                    DateTime logTime = DateTime.MinValue,
                                    requestReceiveTime = DateTime.MinValue,
                                    responseSendTime = DateTime.MinValue;

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
                            if (match.Groups["LogTime"].Success &&
                                DateTime.TryParse(match.Groups["LogTime"].Value, out logTime) &&
                                match.Groups["Millisecond"].Success &&
                                double.TryParse(match.Groups["Millisecond"].Value, out double millisecond))
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

                            methodName = match.Groups["MethodName"].Value;

                            /* 检查方法名称是否在黑名单中
                            if (this.CheckLogInBlackList(methodName))
                            {
                                continue;
                            }
                             */
                            requestReceiveTime = DateTime.TryParse(match.Groups["RequestReceiveTime"].Value, out requestReceiveTime) ? requestReceiveTime : DateTime.MinValue;
                            responseSendTime = DateTime.TryParse(match.Groups["ResponseSendTime"].Value, out responseSendTime) ? responseSendTime : DateTime.MinValue;
                            elapsed = double.TryParse(match.Groups["Elapsed"].Value, out double elapsedValue) ? elapsedValue : 0.0;
                            requestStreamLength = double.TryParse(match.Groups["RequestStreamLength"].Value, out requestStreamLength) ? requestStreamLength : 0.0;
                            responseStreamLength = double.TryParse(match.Groups["ResponseStreamLength"].Value, out responseStreamLength) ? responseStreamLength : 0.0;
                            requestURI = match.Groups["RequestURI"].Value;

                            // 记录所有解析结果
                            PerformanceMonitorResult parseResult = this.CreateParseResult(argument, logFile);
                            parseResult.GroupType = GroupTypes.Unmatch;
                            parseResult.LineNumber = lineNumber;
                            parseResult.LogTime = logTime;
                            parseResult.MethodName = methodName;
                            parseResult.RequestReceiveTime = requestReceiveTime;
                            parseResult.ResponseSendTime = responseSendTime;
                            parseResult.Elapsed = elapsed;
                            parseResult.RequestStreamLength = requestStreamLength;
                            parseResult.ResponseStreamLength = responseStreamLength;
                            parseResult.RequestURI = requestURI;

                            this.ApplyParticularMatch(parseResult, match);

                            // 匹配所有监视规则
                            foreach (PerformanceMonitorItem monitor in monitorItems)
                            {
                                GroupTypes groupType = monitor.MatchGroupLog(methodName);

                                if (groupType == GroupTypes.Unmatch)
                                {
                                    // 未匹配到任何监视规则，尝试下一条规则
                                    continue;
                                }

                                PerformanceMonitorResult result = this.CreateMonitorResultFromParseResult(parseResult, monitor);
                                result.GroupType = groupType;

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
        /// 从解析结果创建监视结果对象
        /// </summary>
        /// <param name="parseResult"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        protected PerformanceMonitorResult CreateMonitorResultFromParseResult(PerformanceMonitorResult parseResult, PerformanceMonitorItem monitor)
        {
            PerformanceMonitorResult monitorResult = parseResult.DeepClone();
            monitorResult.MonitorItem = monitor;

            // 反向关联日志监视结果
            lock (this.lockSeed)
            {
                monitorResult.TaskArgument.PerformanceMonitorResults.Add(monitorResult);
                monitorResult.LogFile.MonitorResults.Add(monitorResult);
                monitorResult.MonitorItem.MonitorResults.Add(monitorResult);
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
        /// 检查日志是否在黑名单中而放弃
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        [Obsolete]
        protected virtual bool CheckLogInBlackList(string log)
            => FixedDatas.MethodNameBlackList.IndexOf(log) > -1;
    }
}
