using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.LogFile;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Parser
{
    /// <summary>
    /// 服务端SQL日志解析器
    /// </summary>
    public class ServerSQLTerminalParser : TerminalLogParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSQLTerminalParser"/> class.
        /// </summary>
        public ServerSQLTerminalParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSQLTerminalParser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ServerSQLTerminalParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志详细内容正则表达式
        /// </summary>
        public override Regex ParticularRegex { get; } = null;

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(TerminalMonitorResult result, Match particularMatch)
        {
        }

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<TerminalLogFile> GetFileFiltered(TaskArgument argument)
            => argument?.TerminalLogFiles.Where(file => file.LogFileType == LogFileTypes.Server && file.LogLevel == LogLevels.SQL);
    }
}
