using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser
{
    /// <summary>
    /// 通用前缀异步组分析器
    /// </summary>
    /// <remarks>此组分析器会自动完成键值对分析，不需要再指定 DirectedAnalysiser="KeyValuePair"</remarks>
    public class FormAsyncGroupAnalysiser : AsyncGroupLogAnalysiserBase
    {
        public FormAsyncGroupAnalysiser()
        {
        }

        public FormAsyncGroupAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

        public override Regex AnalysisRegex { get; protected set; } = new Regex(
            @"(开始|完成)打开窗体\[模块代码=(?<Code>.*?),窗体名称=(?<Name>.*?)\]",
            RegexOptions.RightToLeft | RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            this.Tracer?.WriteLine($"执行 窗体异步组分析器 ....");
            argument.MonitorResults
                .Where(result => result.MonitorItem.GroupAnalysiser == GroupAnalysiserTypes.FormAsync)
                .GroupBy(result => result.MonitorItem)
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key;

                    // 待匹配监视结果寄存字典
                    Dictionary<(string, string), GroupAnalysisResult> unintactResults = new Dictionary<(string, string), GroupAnalysisResult>();
                    GroupAnalysisResult analysisResult = null;
                    Match analysisMatch = null;
                    string formName = string.Empty, moduleCode = string.Empty;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}");
                    foreach (var monitorResult in resultGroup)
                    {
                        lock (this.lockSeed)
                        {
                            analysisMatch = this.AnalysisRegex.Match(monitorResult.LogContent);
                        }
                        formName = analysisMatch.Groups["Name"].Value;
                        moduleCode = analysisMatch.Groups["Code"].Value;

                        // 获取寄存器内未关闭的分析结果
                        unintactResults.TryGetValue((formName, moduleCode), out analysisResult);

                        switch (monitorResult.GroupType)
                        {
                            case GroupTypes.Start:
                                {
                                    // 组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);

                                    analysisResult.AnalysisDatas[FixedDatas.FORM_NAME] = formName;
                                    analysisResult.AnalysisDatas[FixedDatas.MODULE_CODE] = moduleCode;

                                    unintactResults[(formName, moduleCode)] = analysisResult;

                                    break;
                                }
                            case GroupTypes.Finish:
                                {
                                    if (analysisResult != null &&
                                        analysisResult.StartMonitorResult.CheckMatch(monitorResult))
                                    {
                                        // 组匹配类型为Finish时，若存在未关闭的分析结果且结果匹配，则组装分析结果并出队；
                                        analysisResult.FinishMonitorResult = monitorResult;

                                        unintactResults.Remove((formName, moduleCode));
                                    }
                                    else
                                    {
                                        // 不存在同服务名称且同执行序号的分析结果或分析结果不匹配时，新建分析结果
                                        analysisResult = this.CreateAnalysisResult(argument, targetMonitor, monitorResult);

                                        analysisResult.AnalysisDatas[FixedDatas.FORM_NAME] = formName;
                                        analysisResult.AnalysisDatas[FixedDatas.MODULE_CODE] = moduleCode;
                                    }

                                    analysisResult.CalcElapsedMillisecond();
                                    break;
                                }
                        }

                        // Console.WriteLine($"设置分析数据：{coreServiceName}, {index}, {elapsed}; 寄存器数据：{unintactResults.Count} 个");
                    }
                    this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{targetMonitor.AnalysisResults.Count}\t监视规则：{targetMonitor.Name}");
                });
        }
    }
}
