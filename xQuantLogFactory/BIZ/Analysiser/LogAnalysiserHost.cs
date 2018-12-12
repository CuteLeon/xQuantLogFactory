using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser;
using xQuantLogFactory.BIZ.Analysiser.GroupAnalysiser;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
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

        public LogAnalysiserHost()
        {
        }

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

        public override void Analysis(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 优先调用组分析器分析方法
            this.PrepareGroupLogAnalysiser(argument);
            this.GroupAnalysiserProvider.ForEach(analysiser => analysiser.Analysis(argument));

            // 调用定向分析器分析方法
            this.PrepareDirectedLogAnalysiser(argument);
            if (this.DirectedAnalysiserProvider.Count > 0)
            {
                this.DirectedAnalysiserProvider?.ForEach(analysiser => analysiser.Analysis(argument));
            }

            // 分析结果匹配完成后按日志时间排序
            argument.AnalysisResults = argument.AnalysisResults.OrderBy(result => (result.LogTime, result.MonitorItem.CANO)).ToList();

            // 初始化分析结果树
            argument.InitAnalysisResultTree();

            // 计算日志耗时
            this.CalcElapsed(argument);
        }

        /// <summary>
        /// 准备组分析器
        /// </summary>
        /// <param name="argument"></param>
        public void PrepareGroupLogAnalysiser(TaskArgument argument)
        {
            this.GroupAnalysiserProvider.Add(new CommonGroupLogAnalysiser(this.Tracer));

            if (argument.MonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == GroupAnalysiserTypes.CoreServiceAsync))
            {
                this.GroupAnalysiserProvider.Add(new CoreAsyncGroupAnalysiser(this.Tracer));
            }

            if (argument.MonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == GroupAnalysiserTypes.FormAsync))
            {
                this.GroupAnalysiserProvider.Add(new FormAsyncGroupAnalysiser(this.Tracer));
            }

            if (argument.MonitorResults.Any(result => result.MonitorItem.GroupAnalysiser == GroupAnalysiserTypes.ReportAsync))
            {
                this.GroupAnalysiserProvider.Add(new ReportAsyncGroupAnalysiser(this.Tracer));
            }
        }

        /// <summary>
        /// 准备定向分析器
        /// </summary>
        /// <param name="argument"></param>
        public void PrepareDirectedLogAnalysiser(TaskArgument argument)
        {
            var monitorItems = argument.MonitorContainerRoot.GetMonitorItems().ToList();
            if (monitorItems.Count > 0)
            {
                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == DirectedAnalysiserTypes.Prefix))
                {
                    this.AddDirectedAnalysiser(new CommonPrefixAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == DirectedAnalysiserTypes.Load))
                {
                    this.AddDirectedAnalysiser(new CommonLoadAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.DirectedAnalysiser == DirectedAnalysiserTypes.KeyValuePair))
                {
                    this.AddDirectedAnalysiser(new CommonKeyValuePairAnalysiser(this.Tracer));
                }

                if (monitorItems.Any(monitor => monitor.Memory))
                {
                    this.AddDirectedAnalysiser(new CommonMemoryAnalysiser(this.Tracer));
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
    }
}
