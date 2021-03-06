﻿using System;
using System.Text.RegularExpressions;

using LogFactory.Model.Monitor;
using LogFactory.Utils.Trace;

namespace LogFactory.BIZ.Analysiser.DirectedAnalysiser
{
    /// <summary>
    /// 针对性的日志分析器
    /// </summary>
    public abstract class DirectedLogAnalysiserBase : LogAnalysiserBase, ICustomLogAnalysiser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedLogAnalysiserBase"/> class.
        /// </summary>
        public DirectedLogAnalysiserBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedLogAnalysiserBase"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public DirectedLogAnalysiserBase(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// Gets or sets 分析正则
        /// </summary>
        public virtual Regex AnalysisRegex { get; protected set; }

        /// <summary>
        /// 尝试获取或新建子监视规则
        /// </summary>
        /// <param name="parentMonitor">父监视规则</param>
        /// <param name="childMonitorName">子监视规则名称</param>
        /// <returns></returns>
        public virtual TerminalMonitorItem TryGetOrAddChildMonitor(TerminalMonitorItem parentMonitor, string childMonitorName)
        {
            if (parentMonitor == null)
            {
                throw new ArgumentNullException(nameof(parentMonitor));
            }

            lock (parentMonitor)
            {
                TerminalMonitorItem childMonitor = parentMonitor.FindChildMonitorItem(childMonitorName);
                if (childMonitor == null)
                {
                    childMonitor = new TerminalMonitorItem(childMonitorName);

                    // 绑定父节点并复制父节点配置信息
                    childMonitor.BindParentMonitor(parentMonitor, true);
                }

                return childMonitor;
            }
        }
    }
}
