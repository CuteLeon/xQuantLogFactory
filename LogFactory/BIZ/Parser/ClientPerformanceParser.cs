﻿using System;
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
    /// 客户端Performance日志解析器
    /// </summary>
    public class ClientPerformanceParser : PerformanceLogParserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPerformanceParser"/> class.
        /// </summary>
        public ClientPerformanceParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPerformanceParser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ClientPerformanceParser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets 日志总体正则表达式
        /// </summary>
        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(\s(?<ThreadID>\d*)\s)?(?<RequestSendTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<RequestReceiveTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<ResponseSendTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<ResponseReceiveTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<ServerElapsed>\d*?)\s(?<Elapsed>\d*?)\s(?<RequestURI>tcp.*?)\s(?<MethodName>.*?)\s(?<RequestStreamLength>\d*?)\s(?<ResponseStreamLength>\d*?)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected override void ApplyParticularMatch(PerformanceMonitorResult result, Match particularMatch)
        {

            result.RequestSendTime = DateTime.TryParse(particularMatch.Groups["RequestSendTime"].Value, out DateTime requestSendTime) ? requestSendTime : DateTime.MinValue;
            result.RequestReceiveTime = DateTime.TryParse(particularMatch.Groups["RequestReceiveTime"].Value, out DateTime requestReceiveTime) ? requestSendTime : DateTime.MinValue;
            result.ResponseReceiveTime = DateTime.TryParse(particularMatch.Groups["ResponseReceiveTime"].Value, out DateTime responseReceiveTime) ? responseReceiveTime : DateTime.MinValue;
        }

        /// <summary>
        /// 文件过滤
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected override IEnumerable<PerformanceLogFile> GetFileFiltered(TaskArgument argument)
            => argument?.PerformanceLogFiles.Where(file => file.LogFileType == LogFileTypes.Client);
    }
}
