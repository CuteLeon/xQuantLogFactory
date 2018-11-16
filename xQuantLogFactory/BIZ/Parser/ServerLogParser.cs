using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 服务端日志解析器
    /// </summary>
    public class ServerLogParser : CSLogParserBase
    {
        /// <summary>
        /// 日志详细内容正则表达式
        /// </summary>
        public override Regex ParticularRegex { get; } = new Regex(
            @"^(?<Client>.*?)\s(?<Version>.*?(\.[^\s]*){3})\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public ServerLogParser() { }

        public ServerLogParser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<LogFile> GetFileFiltered(TaskArgument argument)
            => argument?.LogFiles.Where(file => file.LogFileType == LogFileTypes.Server);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(MonitorResult result, Match particularMatch)
        {
            result.Version = particularMatch.Groups["Version"].Success ? particularMatch.Groups["Version"].Value : string.Empty;
            result.Client = particularMatch.Groups["Client"].Success ? particularMatch.Groups["Client"].Value : string.Empty;
        }

    }
}
