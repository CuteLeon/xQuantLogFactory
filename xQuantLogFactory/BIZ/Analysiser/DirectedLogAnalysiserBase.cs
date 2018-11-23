using System;
using System.Linq;

using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 针对性的日志分析器
    /// </summary>
    public abstract class DirectedLogAnalysiserBase : LogAnalysiserBase
    {
        public DirectedLogAnalysiserBase()
        {
        }

        public DirectedLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 针对的监视规则名称
        /// </summary>
        public abstract string TargetMonitorName { get; set; }

        /// <summary>
        /// 尝试获取或新建子监视规则
        /// </summary>
        /// <param name="parentMonitor">父监视规则</param>
        /// <param name="childMonitorName">子监视规则名称</param>
        /// <returns></returns>
        public virtual MonitorItem TryGetOrAddChildMonitor(MonitorItem parentMonitor, string childMonitorName)
        {
            if (parentMonitor == null)
            {
                throw new ArgumentNullException(nameof(parentMonitor));
            }

            MonitorItem childMonitor = parentMonitor.MonitorTreeRoots
                .FirstOrDefault(monitor => monitor.Name == childMonitorName);

            if (childMonitor == null)
            {
                childMonitor = new MonitorItem(childMonitorName);
                parentMonitor.MonitorTreeRoots.Add(childMonitor);
            }

            if (childMonitor.ParentMonitorItem == null)
            {
                // TODO: [提醒] 需要赋值父节点配置信息
                childMonitor.ParentMonitorItem = parentMonitor;
                childMonitor.StartPattern = parentMonitor.StartPattern;
                childMonitor.FinishPatterny = parentMonitor.FinishPatterny;
                childMonitor.SheetName = parentMonitor.SheetName;
                childMonitor.Memory = parentMonitor.Memory;
            }

            return childMonitor;
        }
    }
}
