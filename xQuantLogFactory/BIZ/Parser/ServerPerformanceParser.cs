using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 服务端Performance日志解析器
    /// </summary>
    public class ServerPerformanceParser : PerformanceLogParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPerformanceParser"/> class.
        /// </summary>
        public ServerPerformanceParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPerformanceParser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ServerPerformanceParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(\s(?<ThreadID>\d*)\s)?(?<IPAddress>\d{1,3}(\.\d{1,3}){3})\s(?<UserCode>.*?)\s(?<RequestReceiveTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<ResponseSendTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Elapsed>\d*?)\s(?<RequestURI>\/.*?)\s(?<MethodName>.*?)\s(?<RequestStreamLength>\d*?)\s(?<ResponseStreamLength>\d*?)\s(?<Message>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<PerformanceLogFile> GetFileFiltered(TaskArgument argument)
            => argument?.PerformanceLogFiles.Where(file => file.LogFileType == LogFileTypes.Server && file.LogLevel == LogLevels.Perf);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(PerformanceMonitorResult result, Match particularMatch)
        {
            result.IPAddress = particularMatch.Groups["IPAddress"].Value;
            result.UserCode = particularMatch.Groups["UserCode"].Value;
            result.Message = particularMatch.Groups["Message"].Value;
        }
    }
}
