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
    /// 旧版Performance日志解析器
    /// </summary>
    public class OldPerformanceParser : PerformanceLogParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OldPerformanceParser"/> class.
        /// </summary>
        public OldPerformanceParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OldPerformanceParser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public OldPerformanceParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}\.\d{3})\s(?<IPAddress>\d{1,3}(\.\d{1,3}){3})\s(?<UserCode>.*?)\s(?<RequestSendTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}\.\d{3})\s(?<Elapsed>\d*?)\s(?<RequestURI>/.*?)\s(?<MethodName>.*?)\s(?<ResponseStreamLength>\d*?)\s(?<Message>.*?)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(PerformanceMonitorResult result, Match particularMatch)
        {
            result.RequestSendTime = result.LogTime;
            result.IPAddress = particularMatch.Groups["IPAddress"].Value;
            result.UserCode = particularMatch.Groups["UserCode"].Value;
            result.Message = particularMatch.Groups["Message"].Value;
        }

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<PerformanceLogFile> GetFileFiltered(TaskArgument argument)
            => argument?.PerformanceLogFiles.Where(file => file.LogFileType == LogFileTypes.Server && file.LogLevel == LogLevels.PerfOld);
    }
}
