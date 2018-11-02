using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器抽象类
    /// </summary>
    public abstract class LogParserBase : ILogParser
    {

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        /// <remarks>2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券</remarks>
        public abstract Regex LogRegex { get; }

        /// <summary>
        /// 追踪器
        /// </summary>
        public ITrace Trace { get; protected set; }

        public LogParserBase() { }

        public LogParserBase(ITrace trace)
            => this.Trace = trace;

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <returns></returns>
        public abstract IEnumerable<MonitorResult> Parse(TaskArgument argument);

        /// <summary>
        /// 匹配监视规则
        /// </summary>
        /// <param name="monitor">监视规则</param>
        /// <param name="logContent">日志内容</param>
        /// <returns>匹配监视规则类型</returns>
        protected ResultTypes MatchMonitor(MonitorItem monitor, string logContent)
        {
            if (monitor.StartRegex != null && monitor.StartRegex.IsMatch(logContent))
            {
                return ResultTypes.Start;
            }
            else if (monitor.FinishRegex != null && monitor.FinishRegex.IsMatch(logContent))
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
