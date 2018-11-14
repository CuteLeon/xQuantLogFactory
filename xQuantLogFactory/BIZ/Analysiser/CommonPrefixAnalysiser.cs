﻿using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 通用前缀分析器
    /// </summary>
    public class CommonPrefixAnalysiser : DirectedLogAnalysiserBase
    {

        /// <summary>
        /// 针对的监视规则名称
        /// </summary>
        public override string TargetMonitorName { get; set; }

        public CommonPrefixAnalysiser() { }

        public CommonPrefixAnalysiser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 分析监视内容作为前缀的操作日志
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            if (string.IsNullOrEmpty(this.TargetMonitorName))
                throw new ArgumentNullException(nameof(this.TargetMonitorName));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            argument.AnalysisResults
                .Where(result => result.MonitorItem.Name == this.TargetMonitorName)
                .GroupBy(result => (result.LogFile, result.MonitorItem))
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key.MonitorItem;
                    MonitorItem childMonitor = null;
                    MonitorResult firstResult = null;
                    string customeData = string.Empty;
                    string childMonitorName = string.Empty;

                    foreach (var result in resultGroup)
                    {
                        firstResult = result.StartMonitorResult ?? result.FinishMonitorResult;
                        if (firstResult == null) continue;

                        customeData = firstResult.LogContent.Substring((firstResult.GroupType == GroupTypes.Finish ? targetMonitor.FinishPatterny : targetMonitor.StartPattern).Length);
                        childMonitorName = $"{this.TargetMonitorName}-{customeData}";

                        childMonitor = this.TryGetOrAddChildMonitor(targetMonitor, childMonitorName);

                        //更新树状列表版本
                        argument.MonitorItemTree.UpdateVersion();

                        childMonitor.AnalysisResults.Add(result);
                        result.MonitorItem = childMonitor;

                        if (result.StartMonitorResult != null)
                            result.StartMonitorResult.MonitorItem = childMonitor;
                        if (result.FinishMonitorResult != null)
                            result.FinishMonitorResult.MonitorItem = childMonitor;

                        targetMonitor.AnalysisResults.Remove(result);
                    }
                });
        }

        /// <summary>
        /// 尝试获取或新建子监视规则
        /// </summary>
        /// <param name="parentMonitor">父监视规则</param>
        /// <param name="childMonitorName">子监视规则名称</param>
        /// <returns></returns>
        public MonitorItem TryGetOrAddChildMonitor(MonitorItem parentMonitor, string childMonitorName)
        {
            if (parentMonitor == null) throw new ArgumentNullException(nameof(parentMonitor));

            MonitorItem childMonitor = parentMonitor.MonitorItems
                .FirstOrDefault(monitor => monitor.Name == childMonitorName) ??
                new MonitorItem() { Name = childMonitorName };

            if (childMonitor.ParentMonitorItem == null)
            {
                childMonitor.ParentMonitorItem = parentMonitor;
                childMonitor.StartPattern = parentMonitor.StartPattern;
                childMonitor.FinishPatterny = parentMonitor.FinishPatterny;

                parentMonitor.MonitorItems.Add(childMonitor);
            }

            return childMonitor;
        }

    }
}