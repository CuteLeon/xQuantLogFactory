﻿using System;
using System.Linq;

using LogFactory.Model;
using LogFactory.Model.Fixed;
using LogFactory.Model.Monitor;
using LogFactory.Model.Result;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.GroupAnalysiser.Performance
{
    /// <summary>
    /// 通用Performance组分析器
    /// </summary>
    public class CommonPerformanceLogAnalysiser : GroupLogAnalysiserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPerformanceLogAnalysiser"/> class.
        /// </summary>
        public CommonPerformanceLogAnalysiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonPerformanceLogAnalysiser"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public CommonPerformanceLogAnalysiser(ITracer tracer)
            : base(tracer)
        {
        }

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

            /* Performance 日志输出规则：
             * 通过开关控制是否输出事件开始日志；
             * 通过耗时阈值控制是否输出事件完成日志；
             * 解决方案：
             * 仅处理开始标记的Performance日志信息；
             * 解析结果以监视规则、IP地址、用户代码对解析结果分组；
             */
            this.Tracer?.WriteLine($"执行 Performance 通用组分析器 ....");
            argument.PerformanceMonitorResults
                .GroupBy(result => (result.MonitorItem, result.IPAddress, result.UserCode))
                .AsParallel().ForAll(resultGroup =>
                {
                    PerformanceMonitorItem targetMonitor = resultGroup.Key.MonitorItem;
                    string ipAddress = resultGroup.Key.IPAddress;
                    string userCode = resultGroup.Key.UserCode;

                    PerformanceAnalysisResult analysisResult = null;
                    int analysisResultCount = 0;

                    this.Tracer?.WriteLine($">>>开始分析，监视结果数量：{resultGroup.Count()}\t监视规则：{targetMonitor.Name}\tIP地址：{ipAddress}");
                    foreach (var monitorResult in resultGroup)
                    {
                        switch (monitorResult.GroupType)
                        {
                            case GroupTypes.Start:
                                {
                                    // 组匹配类型为Start时，总是新建分析结果并记录监视结果；
                                    analysisResultCount++;
                                    analysisResult = this.CreatePerformanceAnalysisResult(argument, targetMonitor, monitorResult);
                                    break;
                                }

                            case GroupTypes.Finish:
                                {
                                    if (analysisResult == null ||
                                        !analysisResult.StartMonitorResult.CheckMatch(monitorResult))
                                    {
                                        // 组匹配类型为Finish时，若不存在未关闭的分析结果或结果不匹配，则新建分析结果并记录监视结果；
                                        analysisResultCount++;
                                        analysisResult = this.CreatePerformanceAnalysisResult(argument, targetMonitor, monitorResult);
                                    }
                                    else
                                    {
                                        // 组匹配类型为Finish时，若存在未关闭的分析结果，则记录监视结果并计算结果耗时；
                                        analysisResult.FinishMonitorResult = monitorResult;

                                        analysisResult.CalcElapsedMillisecond();
                                    }

                                    // 关闭分析结果，否则会影响下次状态判断
                                    analysisResult = null;
                                    break;
                                }
                        }
                    }

                    this.Tracer?.WriteLine($"<<<分析完成，分析结果数量：{analysisResultCount}\t监视规则：{targetMonitor.Name}\tIP地址：{ipAddress}");
                });
        }
    }
}
