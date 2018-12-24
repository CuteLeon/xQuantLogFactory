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
        /// Initializes a new instance of the <see cref="LogParserBase"/> class.
        /// </summary>
        public LogParserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogParserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
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
    }
}
