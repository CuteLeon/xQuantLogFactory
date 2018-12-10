using System;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser
{
    /// <summary>
    /// 交易清算分析器
    /// </summary>
    public class CommonKeyValuePairAnalysiser : DirectedLogAnalysiserBase
    {
        public CommonKeyValuePairAnalysiser()
        {
        }

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

            argument.AnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == DirectedAnalysiserTypes.KeyValuePair)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;
                    MonitorResult firstResult = null;
                    Match analysisMatch = null;
                    string customeData = string.Empty;

                    foreach (var analysisResult in resultGroup)
                    {
                        firstResult = analysisResult.FirstResultOrDefault();
                        if (firstResult == null)
                        {
                            continue;
                        }

                        analysisMatch = this.AnalysisRegex.Match(firstResult.LogContent);
                        if (analysisMatch.Success && analysisMatch.Groups["Pairs"].Success)
                        {
                            // 匹配日志内容中所有的中括号包含的内容
                            foreach (Match keyValueMatch in this.KeyValuePairRegex.Matches(analysisMatch.Groups["Pairs"].Value))
                            {
                                // 在中括号内容中获取所有键值对数据并录入字典
                                if (keyValueMatch.Groups["Key"].Success && keyValueMatch.Groups["Value"].Success)
                                {
                                    analysisResult.AnalysisDatas[keyValueMatch.Groups["Key"].Value] = keyValueMatch.Groups["Value"].Value;
                                }
                            }
                        }
                    }
                });
        }
    }
}
