﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器抽象类
    /// </summary>
    /// <typeparam name="TLogFile"></typeparam>
    /// <typeparam name="TMonitorResult"></typeparam>
    public abstract class LogParserBase<TLogFile, TMonitorResult> : LogProcesserBase, ILogParser
        where TLogFile : LogFileBase
        where TMonitorResult : MonitorResultBase<TLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogParserBase{TLogFile, TMonitorResult}"/> class.
        /// </summary>
        public LogParserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogParserBase{TLogFile, TMonitorResult}"/> class.
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

        /// <summary>
        /// 获取过滤后文件
        /// </summary>
        /// <typeparam name="TLogFile"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected abstract IEnumerable<TLogFile> GetFileFiltered(TaskArgument argument);

        /// <summary>
        /// 应用精准匹配数据
        /// </summary>
        /// <param name="result"></param>
        /// <param name="particularMatch"></param>
        protected abstract void ApplyParticularMatch(TMonitorResult result, Match particularMatch);
    }
}
