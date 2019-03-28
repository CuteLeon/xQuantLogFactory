using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Result.Transition;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser.Terminal
{
    /// <summary>
    /// 限额检查异步组分析器
    /// </summary>
    public class LimitCheckAsyncGroupAnalysiser : AsyncGroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckAsyncGroupAnalysiser"/> class.
        /// </summary>
        public LimitCheckAsyncGroupAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitCheckAsyncGroupAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public LimitCheckAsyncGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 用户代码、会话ID正则
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
                @"^.*?USER_CODE=(?<UserCode>.*?)，.*?\[(?<SessionID>\d*)\].*$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            Regex tradeCountRegex = new Regex(@"^.*?交易初始化处理前共有交易(?<PreCount>\d*)条,处理后共有交易(?<ProCount>\d*)条，.*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex roleRegex = new Regex(@"^.*?限额条件匹配，(?<RoleCount>\d*)条.*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex resultCountRegex = new Regex(@"^.*?计算完成，共返回(?<ResultCount>\d*)条结果.*$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex totalElapsedRegex = new Regex(@"^.*?耗时合计\s*?\[\d*\]耗时：(?<TotalElapsed>\d*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            this.Tracer?.WriteLine($"执行 限额检查异步组分析器 ....");
            argument.TerminalMonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.LimitCheckAsync)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    var limitChecks = new ConcurrentDictionary<(string userCode, int sessionId), LimitCheckTransition>();
                    var targetMonitor = resultGroup.Key;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");

                    LimitCheckTransition limitCheck = null;
                    foreach (var monitorResult in resultGroup)
                    {
                        var limitCheckKey = this.GetLimitCheckKey(monitorResult.LogContent);
                        if (limitCheckKey.sessionId == null)
                        {
                            continue;
                        }

                        limitCheck = limitChecks.GetOrAdd((limitCheckKey.userCode, limitCheckKey.sessionId.Value), (key) =>
                            new LimitCheckTransition(key.userCode, key.sessionId)
                            {
                                StartMonitorResult = monitorResult,
                            });
                        limitCheck.FinishMonitorResult = monitorResult;

                        GetLimitCheckData(ref limitCheck, monitorResult.LogContent);
                    }

                    limitChecks.Values.ToList().ForEach(limitcheck =>
                    {
                        var analysisResult = this.CreateTerminalAnalysisResult(argument, targetMonitor, limitcheck.StartMonitorResult);
                        limitcheck.ToAnalysisResult(ref analysisResult);
                    });

                    this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{targetMonitor.AnalysisResults.Count}\t监视规则：{targetMonitor.Name}");

                    void GetLimitCheckData(ref LimitCheckTransition limitcheck, string logcontent)
                    {
                        Match match = tradeCountRegex.Match(logcontent);
                        if (match.Success)
                        {
                            limitcheck.PreCount = match.Groups["PreCount"].Success && int.TryParse(match.Groups["PreCount"].Value, out int precount) ? precount : 0;
                            limitcheck.ProCount = match.Groups["ProCount"].Success && int.TryParse(match.Groups["ProCount"].Value, out int procount) ? procount : 0;
                            return;
                        }

                        match = roleRegex.Match(logcontent);
                        if (match.Success)
                        {
                            limitcheck.RoleCount = match.Groups["RoleCount"].Success && int.TryParse(match.Groups["RoleCount"].Value, out int rolecount) ? rolecount : 0;
                            return;
                        }

                        match = resultCountRegex.Match(logcontent);
                        if (match.Success)
                        {
                            limitcheck.ResultCount = match.Groups["ResultCount"].Success && int.TryParse(match.Groups["ResultCount"].Value, out int resultcount) ? resultcount : 0;
                            return;
                        }

                        match = totalElapsedRegex.Match(logcontent);
                        if (match.Success)
                        {
                            limitcheck.TotalElapsed = match.Groups["TotalElapsed"].Success && double.TryParse(match.Groups["TotalElapsed"].Value, out double elapsed) ? elapsed : 0;
                            return;
                        }
                    }
                });
        }

        /// <summary>
        /// 获取会话ID
        /// </summary>
        /// <param name="logContent"></param>
        /// <returns></returns>
        protected (string userCode, int? sessionId) GetLimitCheckKey(string logContent)
        {
            Match match = this.AnalysisRegex.Match(logContent);
            string userCode = match.Success && match.Groups["UserCode"].Success ? match.Groups["UserCode"].Value : null;
            int? sessionId = match.Success && match.Groups["SessionID"].Success && int.TryParse(match.Groups["SessionID"].Value, out int id) ? id : default(int?);
            return (userCode, sessionId);
        }
    }
}
