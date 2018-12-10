using System;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Analysiser.DirectedAnalysiser
{
    /// <summary>
    /// 针对性的日志分析器
    /// </summary>
    public abstract class DirectedLogAnalysiserBase : LogAnalysiserBase, ICustomLogAnalysiser
    {
        public DirectedLogAnalysiserBase()
        {
        }

        public DirectedLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }

        public virtual Regex AnalysisRegex { get; protected set; }

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

            lock (parentMonitor)
            {
                MonitorItem childMonitor = parentMonitor.FindChildMonitorItem(childMonitorName);
                if (childMonitor == null)
                {
                    childMonitor = new MonitorItem(childMonitorName);

                    // 绑定父节点并复制父节点配置信息
                    childMonitor.BindParentMonitor(parentMonitor, true);
                }

                return childMonitor;
            }
        }
    }
}
