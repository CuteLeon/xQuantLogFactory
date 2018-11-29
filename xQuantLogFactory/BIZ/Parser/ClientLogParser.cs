using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 客户端日志解析器
    /// </summary>
    public class ClientLogParser : CSLogParserBase
    {
        public ClientLogParser()
        {
        }

        public ClientLogParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志详细内容正则表达式
        /// </summary>
        public override Regex ParticularRegex { get; } = new Regex(
            @"^(?<Client>.*?)\s(?<Version>.*?(\.[^\s]*){3})\s(?<IPAddress>\d{1,3}(\.\d{1,3}){3})\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<LogFile> GetFileFiltered(TaskArgument argument)
            => argument?.LogFiles.Where(file => file.LogFileType == LogFileTypes.Client);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(MonitorResult result, Match particularMatch)
        {
            result.Version = particularMatch.Groups["Version"].Success ? particularMatch.Groups["Version"].Value : string.Empty;
            result.Client = particularMatch.Groups["Client"].Success ? particularMatch.Groups["Client"].Value : string.Empty;
            result.IPAddress = particularMatch.Groups["IPAddress"].Success ? particularMatch.Groups["IPAddress"].Value : string.Empty;
        }
    }
}
