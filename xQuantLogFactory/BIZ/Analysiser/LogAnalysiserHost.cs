using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器宿主
    /// </summary>
    public abstract class LogAnalysiserHost : LogAnalysiserBase
    {
        /// <summary>
        /// 后续自定义分析器容器
        /// </summary>
        public readonly List<ILogAnalysiser> AnalysiserProvider = new List<ILogAnalysiser>();

        public LogAnalysiserHost()
        {
        }

        public LogAnalysiserHost(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 增加自定义日志分析器
        /// </summary>
        /// <param name="logAnalysiser">日志分析器</param>
        public void AddAnalysiser(ILogAnalysiser logAnalysiser)
        {
            this.AnalysiserProvider?.Add(logAnalysiser);
        }

        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 优先调用宿主分析方法
            this.AnalysisTask(argument);

            // 调用后续自定义分析器分析方法
            this.PrepareDirectedLogAnalysiser(argument);
            if (this.AnalysiserProvider.Count > 0)
            {
                this.AnalysiserProvider?.ForEach(analysiser => analysiser.Analysis(argument));
            }

            // 初始化分析结果树
            this.InitAnalysisResultTree(argument);

            // 计算日志耗时
            this.CalcElapsed(argument);
        }

        /// <summary>
        /// 宿主分析器分析任务
        /// </summary>
        /// <param name="argument"></param>
        public abstract void AnalysisTask(TaskArgument argument);

        /// <summary>
        /// 准备定向分析器
        /// </summary>
        /// <param name="argument"></param>
        public void PrepareDirectedLogAnalysiser(TaskArgument argument)
        {
            var monitorItems = argument.MonitorContainerRoot.GetMonitorItems().ToList();
            if (monitorItems.Count > 0)
            {
                if (monitorItems.Any(monitor => monitor.Analysiser == AnalysiserTypes.Prefix))
                {
                    this.AddAnalysiser(new CommonPrefixAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.Analysiser == AnalysiserTypes.Load))
                {
                    this.AddAnalysiser(new CommonLoadAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.Analysiser == AnalysiserTypes.Settle))
                {
                    this.AddAnalysiser(new TradeSettleAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.Memory))
                {
                    this.AddAnalysiser(new CommonMemoryAnalysiser(this.Tracer));
                }
            }
        }

        /// <summary>
        /// 计算耗时
        /// </summary>
        /// <param name="argument"></param>
        public virtual void CalcElapsed(TaskArgument argument)
        {
            argument.MonitorContainerRoot.GetMonitorItems().ToList().ForEach(monitor =>
            {
                monitor.ElapsedMillisecond = monitor.AnalysisResults.Sum(result => result.ElapsedMillisecond);

                int fullCoupleCount = monitor.AnalysisResults.Count(result => result.IsIntactGroup());
                if (fullCoupleCount > 0)
                {
                    monitor.AverageElapsedMillisecond = monitor.ElapsedMillisecond / fullCoupleCount;
                }
            });

            argument.LogFiles.ForEach(logFile => logFile.ElapsedMillisecond = logFile.AnalysisResults.Sum(result => result.ElapsedMillisecond));
        }

        /// <summary>
        /// 初始化分析结果树
        /// </summary>
        /// <param name="argument"></param>
        public void InitAnalysisResultTree(TaskArgument argument)
        {
            int count = 0;
            foreach (GroupAnalysisResult analysisResult in argument.AnalysisResults)
            {
                Console.WriteLine(++count);
                // 判断分析结果是否为完整组 (内存分析结果为自封闭，开始和结束解析结果相同)
                if (analysisResult.IsIntactGroup() &&
                    analysisResult.StartMonitorResult != analysisResult.FinishMonitorResult)
                {
                    if (analysisResult.MonitorItem.HasChildren)
                    {
                        // 关联子监视规则的分析结果
                        foreach (MonitorItem childMonitor in analysisResult.MonitorItem.MonitorTreeRoots)
                        {
                            var list = childMonitor.AnalysisResults
                                .Where(result => analysisResult.StartMonitorResult.LogTime <= result.LogTime && result.LogTime <= analysisResult.FinishMonitorResult.LogTime)
                                .ToList();
                            list.ForEach(childResult =>
                                {
                                    childResult.ParentAnalysisResult = analysisResult;
                                    analysisResult.AnalysisResultRoots.Add(childResult);
                                });
                        }
                    }

                    // 不存在父级的完整分析组节点记录为分析结果容器的根节点
                    if (analysisResult.ParentAnalysisResult == null)
                    {
                        argument.AnalysisResultContainerRoot.AnalysisResultRoots.Add(analysisResult);
                    }
                }
                else
                {
                    Console.WriteLine($"未入树的分析结果 in {analysisResult.MonitorItem.Name}");
                }
            }
        }
    }
}
