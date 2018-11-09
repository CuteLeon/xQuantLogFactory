using System;
using System.Text.RegularExpressions;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器抽象类
    /// </summary>
    public abstract class LogParserBase : LogProcesserBase, ILogParser
    {

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        public abstract Regex LogRegex { get; }

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
        protected ResultTypes MatchMonitor(MonitorItem monitor, string logContent)
        {
            if (monitor.StartRegex?.IsMatch(logContent) ?? false)
            {
                return ResultTypes.Start;
            }
            else if (monitor.FinishRegex?.IsMatch(logContent) ?? false)
            {
                return ResultTypes.Finish;
            }
            else
            {
                return ResultTypes.Unmatch;
            }
        }

    }
}
