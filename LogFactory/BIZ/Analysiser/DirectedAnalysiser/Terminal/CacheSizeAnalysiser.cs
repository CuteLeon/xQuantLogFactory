﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Extensions;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.DirectedAnalysiser.Terminal
{
    /// <summary>
    /// 统计缓存定向分析器
    /// </summary>
    /// <remarks>暂时无法处理跨文件、穿插、嵌套情况</remarks>
    public class CacheSizeAnalysiser : DirectedLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSizeAnalysiser"/> class.
        /// </summary>
        public CacheSizeAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSizeAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
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

        /// <summary>
        /// Gets or sets 对象缓存正则表达式
        /// </summary>
        public Regex ObjectRegex { get; protected set; } = new Regex(
            $@"\s(?<Type>.*?)\(数量:(?<Count>\d*)\)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.RightToLeft);

        /// <summary>
        /// Gets or sets 拼音缓存正则表达式
        /// </summary>
        public Regex PinYinRegex { get; protected set; } = new Regex(
            $@"拼音缓存个数：(?<Count>\d*);",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 分析任务
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 统计缓存定向分析器 ....");
            argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.CacheSize)
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

                        if (analysisMatch.Groups["Elapsed"].Success &&
                            double.TryParse(analysisMatch.Groups["Elapsed"].Value, out double elapsed))
                        {
                            // 将秒数转换为毫秒数
                            analysisResult.ElapsedMillisecond = elapsed * 1000;
                        }

                        // 分析缓存数据
                        this.AnalysisUnparsedResults(firstResult, analysisResult);
                    }
                });
        }

        /// <summary>
        /// 处理监视结果相关的未解析结果
        /// </summary>
        /// <param name="monitorResult"></param>
        /// <param name="analysisResult"></param>
        private void AnalysisUnparsedResults(TerminalMonitorResult monitorResult, TerminalAnalysisResult analysisResult)
        {
            int startIndex = monitorResult.LogFile.UnparsedResults.FindIndex(result => result.LineNumber >= monitorResult.LineNumber);
            int cacheIndex = monitorResult.LogFile.UnparsedResults.FindIndex(startIndex, result => result.LogContent.IndexOf("缓存对象：", StringComparison.Ordinal) > -1);
            if (cacheIndex == -1)
            {
                // 未找到缓存开始，终止
                return;
            }

            bool hasPinYin = false;
            int pinYinIndex = monitorResult.LogFile.UnparsedResults.FindIndex(cacheIndex + 1, result => result.LogContent.IndexOf("拼音缓存个数：", StringComparison.Ordinal) > -1);
            if (pinYinIndex == -1)
            {
                // 未找到缓存结束，则处理到未解析结果列表最后一个元素
                pinYinIndex = monitorResult.LogFile.UnparsedResults.Count - 1;
            }
            else
            {
                hasPinYin = true;
            }

            for (int index = cacheIndex + 1; index < pinYinIndex; index++)
            {
                var result = monitorResult.LogFile.UnparsedResults[index];

                // 分析对象缓存
                this.AnalysisObjectCount(
                    result,
                    monitorResult.MonitorItem,
                    analysisResult);
            }

            if (hasPinYin)
            {
                // 分析拼音缓存
                this.AnalysisPinYinCount(
                    monitorResult.LogFile.UnparsedResults[pinYinIndex],
                    monitorResult.MonitorItem,
                    analysisResult);
            }
        }

        /// <summary>
        /// 分析对象数量
        /// </summary>
        /// <param name="unparsedResult"></param>
        /// <param name="monitorItem"></param>
        /// <param name="analysisResult"></param>
        private void AnalysisObjectCount(TerminalUnparsedResult unparsedResult, TerminalMonitorItem monitorItem, TerminalAnalysisResult analysisResult)
        {
            Match match = this.ObjectRegex.Match(unparsedResult.LogContent);
            if (match.Success &&
                match.Groups["Type"].Success &&
                match.Groups["Count"].Success &&
                int.TryParse(match.Groups["Count"].Value, out int count))
            {
                string resourceName = match.Groups["Type"].Value ?? "<未知类型>";
                TerminalMonitorItem childMonitor = this.TryGetOrAddChildMonitor(monitorItem, resourceName);

                // 深度克隆原分析结果的开始监视结果并修改行号和日志内容，作为子分析结果的开始监视结果
                TerminalMonitorResult childResult = analysisResult.StartMonitorResult.DeepClone();
                childResult.LineNumber = unparsedResult.LineNumber;
                childResult.LogContent = unparsedResult.LogContent;

                // 创建子分析结果
                TerminalAnalysisResult childAnalysisResult = this.CreateTerminalAnalysisResult(
                    analysisResult.TaskArgument,
                    childMonitor,
                    childResult);

                childAnalysisResult.AnalysisDatas.Add(FixedDatas.RESOURCE_NAME, resourceName);
                childAnalysisResult.AnalysisDatas.Add(FixedDatas.COUNT, count);
            }
        }

        /// <summary>
        /// 分析拼音数量
        /// </summary>
        /// <param name="unparsedResult"></param>
        /// <param name="monitorItem"></param>
        /// <param name="analysisResult"></param>
        private void AnalysisPinYinCount(TerminalUnparsedResult unparsedResult, TerminalMonitorItem monitorItem, TerminalAnalysisResult analysisResult)
        {
            Match match = this.PinYinRegex.Match(unparsedResult.LogContent);
            if (match.Success &&
                match.Groups["Count"].Success &&
                int.TryParse(match.Groups["Count"].Value, out int count))
            {
                TerminalMonitorItem childMonitor = this.TryGetOrAddChildMonitor(monitorItem, "拼音缓存");

                // 深度克隆原分析结果的开始监视结果并修改行号和日志内容，作为子分析结果的开始监视结果
                TerminalMonitorResult childResult = analysisResult.StartMonitorResult.DeepClone();
                childResult.LineNumber = unparsedResult.LineNumber;
                childResult.LogContent = unparsedResult.LogContent;

                // 创建子分析结果
                TerminalAnalysisResult childAnalysisResult = this.CreateTerminalAnalysisResult(
                    analysisResult.TaskArgument,
                    childMonitor,
                    childResult);

                childAnalysisResult.AnalysisDatas.Add(FixedDatas.RESOURCE_NAME, "拼音");
                childAnalysisResult.AnalysisDatas.Add(FixedDatas.COUNT, count);
            }
        }
    }
}
