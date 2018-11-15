using System;
using System.Text.RegularExpressions;

using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 客户端和服务端日志解析抽象类
    /// </summary>
    public abstract class CSLogParserBase : LogParserBase
    {

        public CSLogParserBase() { }

        public CSLogParserBase(ITracer trace) : base(trace) { }

        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 内存信息正则表达式
        /// </summary>
        public virtual Regex MemoryRegex { get; } = new Regex(
            @"内存消耗：.*?VirtualMem=(?<Memory>\d*\.\d*).*",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

        /// <summary>
        /// 日志详细内容正则表达式
        /// </summary>
        public abstract Regex ParticularRegex { get; }

        /// <summary>
        /// 从日志内容获取消耗内存
        /// </summary>
        /// <param name="logContent"></param>
        /// <returns></returns>
        protected double GetMemoryInLogContent(string logContent)
        {
            if (string.IsNullOrEmpty(logContent)) return 0.0;

            Match memoryMatch = this.MemoryRegex.Match(logContent);

            if (memoryMatch.Success &&
                memoryMatch.Groups["Memory"].Success &&
                double.TryParse(memoryMatch.Groups["Memory"].Value, out double memory)
                )
            {
                return memory;
            }
            else
            {
                return 0.0;
            }
        }

    }
}
