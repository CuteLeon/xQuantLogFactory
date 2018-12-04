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
            // TODO: 无法稳定复现的问题：初始化树并通过栈扫描时，扫描结果列表会比真实分析结果列表多 n(n>=0) 个
            foreach (GroupAnalysisResult analysisResult in argument.AnalysisResults)
            {
                // 判断分析结果是否为完整组
                if (analysisResult.IsIntactGroup())
                {
                    if (analysisResult.MonitorItem.HasChildren)
                    {
                        // 关联子监视规则的分析结果（以当前节点为树根遍历监视规则树，防止分析结果之间断级，而造成分析结果树断开）
                        foreach (MonitorItem childMonitor in analysisResult.MonitorItem.GetMonitorItems())
                        {
                            childMonitor.AnalysisResults
                                .Where(result =>
                                    analysisResult.StartMonitorResult.LogTime <= result.LogTime &&
                                    result.LogTime <= analysisResult.FinishMonitorResult.LogTime)
                                .ToList().ForEach(childResult =>
                                {
                                    // 节约性能，仅关联父节点，防止重复高频的List操作
                                    childResult.ParentAnalysisResult = analysisResult;
                                });
                        }
                    }

                    // 不存在父级的完整分析组节点记录为分析结果容器的根节点
                    if (analysisResult.ParentAnalysisResult == null)
                    {
                        argument.AnalysisResultContainerRoot.AnalysisResultRoots.Add(analysisResult);
                    }
                }
            }

            // 根据分析结果父级节点反向关联子分析结果，单独处理，防止重复高频操作
            foreach (var analysisResult in argument.AnalysisResults)
            {
                if (analysisResult.ParentAnalysisResult != null)
                {
                    analysisResult.ParentAnalysisResult.AnalysisResultRoots.Add(analysisResult);
                }
            }
        }
    }
}
