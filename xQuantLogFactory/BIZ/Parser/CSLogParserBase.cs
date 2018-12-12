﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 客户端和服务端日志解析抽象类
    /// </summary>
    public abstract class CSLogParserBase : LogParserBase
    {
        public CSLogParserBase()
        {
        }

        public CSLogParserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<LogContent>.+)$",
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
                this.Tracer?.WriteLine($"开始解析日志文件：(ID: {logFile.RelativePath}, Type: {logFile.LogFileType}) {logFile.RelativePath}");

                FileStream fileStream = null;
                StreamReader streamRreader = null;
                try
                {
                    fileStream = new FileStream(logFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    streamRreader = new StreamReader(fileStream, Encoding.Default);

                    int lineNumber = 0;
                    string logLine = string.Empty,
                        logLevel = string.Empty,
                        logContent = string.Empty;
                    Match generalMatch = null;
                    Match particularMatch = null;

                    // 缓存变量，减少树扫描次数，需要 .ToList()
                    List<MonitorItem> monitorItems = null;
                    lock (argument.MonitorContainerRoot)
                    {
                        monitorItems = argument.MonitorContainerRoot.GetMonitorItems().ToList();
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

                            if (!argument.CheckLogStartTime(logTime) ||
                                !argument.CheckLogFinishTime(logTime))
                            {
                                continue;
                            }

                            logContent = generalMatch.Groups["LogContent"].Value;
                            logLevel = generalMatch.Groups["LogLevel"].Success ? generalMatch.Groups["LogLevel"].Value : string.Empty;

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

                            // 匹配所有监视规则
                            foreach (MonitorItem monitor in monitorItems)
                            {
                                GroupTypes groupType = monitor.MatchLogContent(logContent);
                                if (groupType == GroupTypes.Unmatch)
                                {
                                    // 未匹配到任何监视规则，尝试下一条规则
                                    continue;
                                }

                                MonitorResult result = this.CreateMonitorResult(argument, logFile, monitor);
                                result.LogTime = logTime;
                                result.GroupType = groupType;
                                result.LineNumber = lineNumber;
                                result.LogContent = logContent;
                                result.LogLevel = logLevel;

                                // 应用精准匹配数据
                                if (particularMatch.Success)
                                {
                                    this.ApplyParticularMatch(result, particularMatch);
                                }

                                this.Tracer.WriteLine($"发现监视结果：\n\t文件ID= {logFile.RelativePath} 行号= {result.LineNumber} 等级= {result.LogLevel} 日志内容= {result.LogContent}");
                            }
                        }
                        else
                        {
                            // 未识别的日志行
                        }
                    }

                    this.Tracer?.WriteLine($"当前日志文件(ID: {logFile.RelativePath})解析完成\n————————");
                }
                catch (Exception ex)
                {
                    this.Tracer?.WriteLine($"解析日志文件(ID: {logFile.RelativePath}) {logFile.RelativePath} 失败：{ex.Message}\n————————");
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
            argument.MonitorResults = argument.MonitorResults.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();
        }

        /// <summary>
        /// 获取过滤后文件
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected abstract IEnumerable<LogFile> GetFileFiltered(TaskArgument argument);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected abstract void ApplyParticularMatch(MonitorResult result, Match particularMatch);
    }
}
