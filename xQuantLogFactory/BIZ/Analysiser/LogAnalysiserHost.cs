using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser;
using xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器宿主
    /// </summary>
    public class LogAnalysiserHost : LogAnalysiserBase
    {
        /// <summary>
        /// 组分析器容器
        /// </summary>
        public readonly List<GroupLogAnalysiserBase> GroupAnalysiserProvider = new List<GroupLogAnalysiserBase>();

        /// <summary>
        /// 定向分析器容器
        /// </summary>
        public readonly List<DirectedLogAnalysiserBase> DirectedAnalysiserProvider = new List<DirectedLogAnalysiserBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAnalysiserHost"/> class.
        /// </summary>
        public LogAnalysiserHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAnalysiserHost"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public LogAnalysiserHost(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 增加定向日志分析器
        /// </summary>
        /// <param name="logAnalysiser">日志分析器</param>
        public void AddDirectedAnalysiser(DirectedLogAnalysiserBase logAnalysiser)
        {
            this.DirectedAnalysiserProvider?.Add(logAnalysiser);
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

            // 优先调用组分析器分析方法
            this.Tracer?.WriteLine(">>>————— 准备组分析器 —————");
            this.PrepareGroupLogAnalysiser(argument);
            this.Tracer?.WriteLine(">>>执行组分析器 ...");
            this.GroupAnalysiserProvider.ForEach(analysiser => analysiser.Analysis(argument));

            // 调用定向分析器分析方法
            this.Tracer?.WriteLine(">>>————— 准备定向分析器 —————");
            this.PrepareDirectedLogAnalysiser(argument);
            if (this.DirectedAnalysiserProvider.Count > 0)
            {
                this.Tracer?.WriteLine(">>>执行定向分析器 ...");
                this.DirectedAnalysiserProvider?.ForEach(analysiser => analysiser.Analysis(argument));
            }

            // 分析结果匹配完成后按日志时间排序
            this.Tracer?.WriteLine(">>>————— 分析结果池排序 —————");
            argument.TerminalAnalysisResults = argument.TerminalAnalysisResults.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();
            argument.PerformanceAnalysisResults = argument.PerformanceAnalysisResults.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();
            this.Tracer?.WriteLine("<<< 排序完成");

            // 初始化分析结果树
            this.Tracer?.WriteLine(">>>————— 构建分析结果树 —————");
            argument.InitAnalysisResultTree();
            this.Tracer?.WriteLine("<<< 构建完成");

            // 统计分析结果
            this.Tracer?.WriteLine(">>>————— 统计分析结果 —————");
            this.FigureOutAnalysisResults(argument);
            this.Tracer?.WriteLine("<<< 统计完成");
        }

        /// <summary>
        /// 准备组分析器
        /// </summary>
        /// <param name="argument"></param>
        public void PrepareGroupLogAnalysiser(TaskArgument argument)
        {
            if (argument.TerminalMonitorResults.Count > 0)
            {
                this.GroupAnalysiserProvider.Add(new CommonGroupLogAnalysiser(this.Tracer));

                if (argument.TerminalMonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.CoreServiceAsync))
                {
                    this.GroupAnalysiserProvider.Add(new CoreAsyncGroupAnalysiser(this.Tracer));
                }

                if (argument.TerminalMonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.FormAsync))
                {
                    this.GroupAnalysiserProvider.Add(new FormAsyncGroupAnalysiser(this.Tracer));
                }

                if (argument.TerminalMonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.ReportAsync))
                {
                    this.GroupAnalysiserProvider.Add(new ReportAsyncGroupAnalysiser(this.Tracer));
                }

                if (argument.TerminalMonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.SelfSealing))
                {
                    this.GroupAnalysiserProvider.Add(new CommonSelfSealingGroupAnalysiser(this.Tracer));
                }
            }

            if (argument.PerformanceMonitorResults.Count > 0)
            {
                // TODO: 准备 Performance 组分析器
            }
        }

        /// <summary>
        /// 准备定向分析器
        /// </summary>
        /// <param name="argument"></param>
        public void PrepareDirectedLogAnalysiser(TaskArgument argument)
        {
            var monitorItems = argument.MonitorContainerRoot.GetTerminalMonitorItems().ToList();
            if (monitorItems.Count > 0)
            {
                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.Prefix))
                {
                    this.AddDirectedAnalysiser(new CommonPrefixAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.Load))
                {
                    this.AddDirectedAnalysiser(new CommonLoadAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.KeyValuePair))
                {
                    this.AddDirectedAnalysiser(new CommonKeyValuePairAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.Memory))
                {
                    this.AddDirectedAnalysiser(new CommonMemoryAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.CacheSize))
                {
                    this.AddDirectedAnalysiser(new CacheSizeAnalysiser(this.Tracer));
                }
            }
        }

        /// <summary>
        /// 统计分析结果
        /// </summary>
        /// <param name="argument"></param>
        public virtual void FigureOutAnalysisResults(TaskArgument argument)
        {
            this.Tracer?.WriteLine("未完全匹配的监视规则：");

            foreach (var monitor in argument.MonitorContainerRoot.GetTerminalMonitorItems()
                .Where(monitor => monitor.MonitorResults.Count > 0))
            {
                double matchingRate = monitor.MatchingRate;
                if (matchingRate < 1.0)
                {
                    this.Tracer?.WriteLine($"匹配率：{matchingRate.ToString("P2")}\t监视规则：{monitor.Name}");
                }
            }

            foreach (var monitor in argument.MonitorContainerRoot.GetPerformanceMonitorItems()
                .Where(monitor => monitor.MonitorResults.Count > 0))
            {
                double matchingRate = monitor.MatchingRate;
                if (matchingRate < 1.0)
                {
                    this.Tracer?.WriteLine($"匹配率：{matchingRate.ToString("P2")}\t监视规则：{monitor.Name}");
                }
            }
        }
    }
}
