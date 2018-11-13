using System;
using System.Linq;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 窗口操作分析器
    /// </summary>
    public class FormOperationAnalysiser : DirectedLogAnalysiserBase
    {
        //根据监视规则名称查找窗口操作相关分析结果，获取分析结果所在窗口名称，根据窗口名称为监视规则新建子监视规则，并将分析结果赋值给子监视规则

        /// <summary>
        /// 针对的监视规则名称
        /// </summary>
        protected override string TargetMonitorName { get; set; } = "开关窗体";

        public FormOperationAnalysiser() { }

        public FormOperationAnalysiser(ITracer trace) : base(trace) { }

        /// <summary>
        /// 分析窗口操作日志
        /// </summary>
        /// <param name="argument"></param>
        public override void Analysis(TaskArgument argument)
        {
            argument.AnalysisResults
                .Where(result => result.MonitorItem.Name == this.TargetMonitorName)
                .GroupBy(result => (result.LogFile, result.MonitorItem))
                .AsParallel().ForAll(resultGroup =>
                {
                    MonitorItem targetMonitor = resultGroup.Key.MonitorItem;
                    MonitorItem childMonitor = null;
                    MonitorResult firstResult = null;
                    string formName = string.Empty;
                    string childMonitorName = string.Empty;

                    foreach (var result in resultGroup)
                    {
                        firstResult = result.StartMonitorResult ?? result.FinishMonitorResult;
                        if (firstResult == null) continue;

                        formName = firstResult.LogContent.Substring((firstResult.GroupType == GroupTypes.Finish ? targetMonitor.FinishPatterny : targetMonitor.StartPattern).Length);
                        childMonitorName = $"{this.TargetMonitorName}-{formName}";

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
