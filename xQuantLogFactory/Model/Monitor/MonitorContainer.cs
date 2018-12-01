using System;
using System.Linq;
using System.Xml.Serialization;

using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("MonitorRoot")]
    public class MonitorContainer : MonitorBase
    {
        /// <summary>
        /// 初始化监视规则树
        /// </summary>
        public void InitMonitorTree()
        {
            // 初始化当前节点的第一层子节点
            MonitorItem childMonitor = null;
            for (int index = 0; index < this.MonitorTreeRoots.Count; index++)
            {
                childMonitor = this.MonitorTreeRoots[index];

                // 初始化默认表名
                if (string.IsNullOrEmpty(childMonitor.SheetName))
                {
                    childMonitor.SheetName = ConfigHelper.ExcelSourceSheetName;
                }

                // 当监视规则开始条件为空时，使用前一兄弟规则?.结束条件；
                if (string.IsNullOrEmpty(childMonitor.StartPattern)
                    && index > 0
                    && !string.IsNullOrEmpty(this.MonitorTreeRoots[index - 1].FinishPatterny))
                {
                    childMonitor.StartPattern = this.MonitorTreeRoots[index - 1].FinishPatterny;
                }
            }

            this.ScanMonitor(
                (rootStack, currentMonitor) =>
                {
                    currentMonitor.MonitorTreeRoots
                    .AsEnumerable().Reverse()
                    .ToList().ForEach(monitor =>
                    {
                        // 复制父级节点配置
                        monitor.BindParentMonitor(currentMonitor);

                        // 包含子节点的节点继续入栈
                        if (monitor.HasChildren)
                        {
                            rootStack.Push(monitor);

                            // 当监视规则开始条件为空时，使用前一兄弟规则?.结束条件；
                            for (int index = 0; index < monitor.MonitorTreeRoots.Count; index++)
                            {
                                childMonitor = monitor.MonitorTreeRoots[index];

                                if (string.IsNullOrEmpty(childMonitor.StartPattern)
                                && index > 0
                                && !string.IsNullOrEmpty(monitor.MonitorTreeRoots[index - 1].FinishPatterny))
                                {
                                    childMonitor.StartPattern = monitor.MonitorTreeRoots[index - 1].FinishPatterny;
                                }
                            }
                        }
                    });
                },
                null);
        }
    }
}
