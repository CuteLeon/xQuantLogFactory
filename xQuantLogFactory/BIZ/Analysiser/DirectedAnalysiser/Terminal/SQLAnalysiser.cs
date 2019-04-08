using System;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// SQL分析器
    /// </summary>
    public class SQLAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLAnalysiser"/> class.
        /// </summary>
        public SQLAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public SQLAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"^.*?=>(?<DBUser>.*?)\s(?<SQLHash>.*?)\s(?<RowCount>-?\d*?)\s(?<StartTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<FinishTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Elapsed>\d*?)\s(?<Params>.*?)$",
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

            this.Tracer?.WriteLine($"执行 SQL定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.SQL)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;
                    TerminalMonitorResult firstResult = null;
                    Match analysisMatch = null;

                    this.Tracer?.WriteLine($">>>正在分析监视规则：{targetMonitor.Name}，结果数量：{resultGroup.Count()}");
                    foreach (var analysisResult in resultGroup)
                    {
                        firstResult = analysisResult.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        lock (this.AnalysisRegex)
                        {
                            analysisMatch = this.AnalysisRegex.Match(firstResult.LogContent);
                        }

                        if (analysisMatch.Success)
                        {
                            analysisResult.AnalysisDatas[FixedDatas.DATABASE] = analysisMatch.Groups["DBUser"].Success ? analysisMatch.Groups["DBUser"].Value : string.Empty;
                            analysisResult.AnalysisDatas[FixedDatas.HASH] = analysisMatch.Groups["SQLHash"].Success ? analysisMatch.Groups["SQLHash"].Value : string.Empty;
                            analysisResult.AnalysisDatas[FixedDatas.RESULT_COUNT] = Math.Max(analysisMatch.Groups["RowCount"].Success && int.TryParse(analysisMatch.Groups["RowCount"].Value, out int count) ? count : 0, 0);
                            analysisResult.AnalysisDatas[FixedDatas.PARAMS] = analysisMatch.Groups["Params"].Success ? analysisMatch.Groups["Params"].Value : string.Empty;

                            analysisResult.ElapsedMillisecond = analysisMatch.Groups["Elapsed"].Success && double.TryParse(analysisMatch.Groups["Elapsed"].Value, out double elapsed) ? elapsed : 0;
                        }
                    }
                });
        }
    }
}
