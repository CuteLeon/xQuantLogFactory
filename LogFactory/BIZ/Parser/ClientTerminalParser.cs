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
    /// 客户端日志解析器
    /// </summary>
    public class ClientTerminalParser : TerminalLogParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTerminalParser"/> class.
        /// </summary>
        public ClientTerminalParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientTerminalParser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ClientTerminalParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志详细内容正则表达式
        /// </summary>
        public override Regex ParticularRegex { get; } = new Regex(
            @"^(?<Client>.*?)\s(?<Version>.*?)\s(?<IPAddress>\d{1,3}(\.\d{1,3}){3})\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<TerminalLogFile> GetFileFiltered(TaskArgument argument)
            => argument?.TerminalLogFiles.Where(file => file.LogFileType == LogFileTypes.Client);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(TerminalMonitorResult result, Match particularMatch)
        {
            result.Version = particularMatch.Groups["Version"].Success ? particularMatch.Groups["Version"].Value : string.Empty;
            result.Client = particularMatch.Groups["Client"].Success ? particularMatch.Groups["Client"].Value : string.Empty;
            result.IPAddress = particularMatch.Groups["IPAddress"].Success ? particularMatch.Groups["IPAddress"].Value : string.Empty;
        }
    }
}
