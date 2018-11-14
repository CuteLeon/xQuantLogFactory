﻿using System;
using System.Text.RegularExpressions;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器抽象类
    /// </summary>
    public abstract class LogParserBase : LogProcesserBase, ILogParser
    {
        //TODO: 具体子类解析器字段格式（版本号加 \.），因为有些日志行依然被 ParticularRegex 识别成功，导致匹配失败（分析匹配时会检查版本和客户端）
        //TODO: 提取子类中 GeneralLogRegex 匹配的公共代码到此基类（MiddlewareLogParser 不使用此正则表达式对象）

        /// <summary>
        /// 日志详细内容正则表达式
        /// </summary>
        public abstract Regex ParticularRegex { get; }

        /// <summary>
        /// 日志总体正则表达式
        /// </summary>
        public Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public LogParserBase() { }

        public LogParserBase(ITracer trace) : base(trace) { }

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        public abstract void Parse(TaskArgument argument);

        /// <summary>
        /// 匹配监视规则
        /// </summary>
        /// <param name="monitor">监视规则</param>
        /// <param name="logContent">日志内容</param>
        /// <returns>匹配监视规则类型</returns>
        protected GroupTypes MatchMonitor(MonitorItem monitor, string logContent)
        {
            //以下字符串判空方法会获得比 ""==string.Empty 更好的性能
            if (monitor.StartPattern?.Length > 0 &&
                logContent.IndexOf(monitor.StartPattern, StringComparison.Ordinal) > -1)
            {
                return GroupTypes.Start;
            }
            else if (monitor.FinishPatterny?.Length > 0 &&
                logContent.IndexOf(monitor.FinishPatterny, StringComparison.Ordinal) > -1)
            {
                return GroupTypes.Finish;
            }
            else
            {
                return GroupTypes.Unmatch;
            }
        }

        /// <summary>
        /// 创建解析结果对象
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        protected MonitorResult CreateMonitorResult(TaskArgument argument, LogFile logFile, MonitorItem monitor)
        {
            MonitorResult monitorResult = new MonitorResult();

            //反向关联日志监视结果
            lock (argument)  //lock 任务而非 LockSeed 为了多任务并行考虑
            {
                monitorResult.TaskArgument = argument;
                monitorResult.LogFile = logFile;
                monitorResult.MonitorItem = monitor;

                argument.MonitorResults.Add(monitorResult);
                logFile.MonitorResults.Add(monitorResult);
                monitor.MonitorResults.Add(monitorResult);
            }

            return monitorResult;
        }

    }
}
