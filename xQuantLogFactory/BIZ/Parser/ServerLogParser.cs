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
    /// 服务端日志解析器
    /// </summary>
    public class ServerLogParser : CSLogParserBase
    {
        public ServerLogParser()
        {
        }

        public ServerLogParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志详细内容正则表达式
        /// </summary>
        /// <remarks>日志内容存在", "(英文逗号加空格)时会被正则错误分析，需确保正常的客户端名称和版本号不以英文逗号结尾</remarks>
        public override Regex ParticularRegex { get; } = new Regex(
            @"^(?<Client>.*?[^,])\s(?<Version>.*?[^,])\s(?<LogContent>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

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
