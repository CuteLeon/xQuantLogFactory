using System;
using System.Text.RegularExpressions;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
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
        public LogParserBase()
        {
        }

        public LogParserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public abstract Regex GeneralLogRegex { get; }

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
            // 以下字符串判空方法会获得比 ""==string.Empty 更好的性能
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
            MonitorResult monitorResult = new MonitorResult(argument, logFile, monitor);

            // 反向关联日志监视结果
            lock (argument)
            {
                argument.MonitorResults.Add(monitorResult);
                logFile.MonitorResults.Add(monitorResult);
                monitor.MonitorResults.Add(monitorResult);
            }

            return monitorResult;
        }
    }
}
