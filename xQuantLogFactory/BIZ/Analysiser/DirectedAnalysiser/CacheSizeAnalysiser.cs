using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Extensions;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser
{
    /// <summary>
    /// 统计缓存定向分析器
    /// </summary>
    public class CacheSizeAnalysiser : DirectedLogAnalysiserBase
    {
        public CacheSizeAnalysiser()
        {
        }

        public CacheSizeAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则表达式
        /// </summary>
        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            $@"统计耗时：(?<Elapsed>.*?)秒",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public Regex PinYinRegex { get; protected set; } = new Regex(
            $@"拼音缓存个数：(?<Count>\d*);",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 统计缓存定向分析器 ....");
            argument.AnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == DirectedAnalysiserTypes.CacheSize)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;
                    MonitorResult firstResult = null;
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

                        if (analysisMatch.Groups["Elapsed"].Success &&
                            double.TryParse(analysisMatch.Groups["Elapsed"].Value, out double elapsed))
                        {
                            // 将秒数转换为毫秒数
                            analysisResult.ElapsedMillisecond = elapsed * 1000;
                        }

                        // 分析缓存数据
                        foreach (var result in this.GetUnparsedResults(firstResult, analysisResult))
                        {
                            // TODO: 分析对象缓存数量
                            Console.WriteLine(result.LogContent);
                        }
                    }
                });
        }

        /// <summary>
        /// 获取监视结果相关的未解析结果
        /// </summary>
        /// <param name="monitorResult"></param>
        /// <param name="analysisResult"></param>
        /// <returns></returns>
        private IEnumerable<UnparsedResult> GetUnparsedResults(MonitorResult monitorResult, GroupAnalysisResult analysisResult)
        {
            int startIndex = monitorResult.LogFile.UnparsedResults.FindIndex(result => result.LineNumber >= monitorResult.LineNumber);
            int cacheIndex = monitorResult.LogFile.UnparsedResults.FindIndex(startIndex, result => result.LogContent.IndexOf("缓存对象：", StringComparison.Ordinal) > -1);
            if (cacheIndex == -1)
            {
                // 未找到缓存开始，终止
                yield break;
            }

            int pinYinIndex = monitorResult.LogFile.UnparsedResults.FindIndex(cacheIndex + 1, result => result.LogContent.IndexOf("拼音缓存个数：", StringComparison.Ordinal) > -1);
            if (pinYinIndex == -1)
            {
                // 未找到缓存结束，则处理到未解析结果列表最后一个元素
                pinYinIndex = monitorResult.LogFile.UnparsedResults.Count - 1;
            }
            else
            {
                // 分析拼音缓存
                this.AnalysisPinYinCount(
                    monitorResult.LogFile.UnparsedResults[pinYinIndex],
                    monitorResult.MonitorItem,
                    analysisResult);
            }

            for (int index = cacheIndex + 1; index < pinYinIndex; index++)
            {
                yield return monitorResult.LogFile.UnparsedResults[index];
            }
        }

        /// <summary>
        /// 分析拼音数量
        /// </summary>
        /// <param name="unparsedResult"></param>
        /// <param name="monitorItem"></param>
        /// <param name="analysisResult"></param>
        private void AnalysisPinYinCount(UnparsedResult unparsedResult, MonitorItem monitorItem, GroupAnalysisResult analysisResult)
        {
            Match pinYinMatch = this.PinYinRegex.Match(unparsedResult.LogContent);
            if (pinYinMatch.Success &&
                pinYinMatch.Groups["Count"].Success &&
                int.TryParse(pinYinMatch.Groups["Count"].Value, out int count))
            {
                MonitorItem childMonitor = this.TryGetOrAddChildMonitor(monitorItem, "拼音缓存");

                // 深度克隆原分析结果的开始监视结果并修改行号和日志内容，作为子分析结果的开始监视结果
                MonitorResult childResult = analysisResult.StartMonitorResult.DeepClone();
                childResult.LineNumber = unparsedResult.LineNumber;
                childResult.LogContent = unparsedResult.LogContent;

                // 创建子分析结果
                GroupAnalysisResult childAnalysisResult = this.CreateAnalysisResult(
                    analysisResult.TaskArgument,
                    childMonitor,
                    childResult);

                childAnalysisResult.AnalysisDatas.Add(FixedDatas.RESOURCE_NAME, "拼音");
                childAnalysisResult.AnalysisDatas.Add(FixedDatas.COUNT, count);
            }
        }
    }
}
