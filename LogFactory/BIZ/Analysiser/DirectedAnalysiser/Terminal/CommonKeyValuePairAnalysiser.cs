using System;
using System.Linq;
using System.Text.RegularExpressions;

using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// 交易清算分析器
    /// </summary>
    public class CommonKeyValuePairAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonKeyValuePairAnalysiser"/> class.
        /// </summary>
        public CommonKeyValuePairAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonKeyValuePairAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonKeyValuePairAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则表达式
        /// </summary>
        /// <remarks>中括号可以多次出现，但是不允许嵌套或残缺</remarks>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            $@"\[(?<Pairs>.*?)\]",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Gets or sets 键值对分析器
        /// </summary>
        /// <remarks>键和值不允许出现英文逗号和空格</remarks>
        public virtual Regex KeyValuePairRegex { get; protected set; } = new Regex(
            $@"(?<Key>[^,\s]+)=(?<Value>[^,\s]*)",
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

            this.Tracer?.WriteLine($"执行 通用键值对定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.KeyValuePair)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    TerminalMonitorItem targetMonitor = resultGroup.Key;
                    Match analysisMatch = null;

                    this.Tracer?.WriteLine($">>>正在分析监视规则：{targetMonitor.Name}，结果数量：{resultGroup.Count()}");
                    foreach (var analysisResult in resultGroup)
                    {
                        // 先分析结束监视结果，后分析开始监视结果：使用开始监视结果的键值对数据覆盖分析数据字典
                        MatchLogContent(analysisResult.FinishMonitorResult?.LogContent);
                        MatchLogContent(analysisResult.StartMonitorResult?.LogContent);

                        void MatchLogContent(string logContent)
                        {
                            if (string.IsNullOrEmpty(logContent))
                            {
                                return;
                            }

                            lock (this.AnalysisRegex)
                            {
                                analysisMatch = this.AnalysisRegex.Match(logContent);
                            }

                            if (analysisMatch.Success && analysisMatch.Groups["Pairs"].Success)
                            {
                                MatchCollection matchs = null;
                                // 匹配日志内容中所有的中括号包含的内容
                                lock (this.KeyValuePairRegex)
                                {
                                    matchs = this.KeyValuePairRegex.Matches(analysisMatch.Groups["Pairs"].Value);
                                }

                                foreach (Match match in matchs)
                                {
                                    // 在中括号内容中获取所有键值对数据并录入字典
                                    if (match.Groups["Key"].Success && match.Groups["Value"].Success)
                                    {
                                        analysisResult.AnalysisDatas[match.Groups["Key"].Value] = match.Groups["Value"].Value;
                                    }
                                }
                            }
                        }
                    }
                });
        }
    }
}
