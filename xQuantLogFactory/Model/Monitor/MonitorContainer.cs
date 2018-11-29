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
            this.MonitorTreeRoots.ForEach(childMonitor =>
            {
                if (string.IsNullOrEmpty(childMonitor.SheetName))
                {
                    childMonitor.SheetName = ConfigHelper.ExcelSourceSheetName;
                }
            });

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
                        }
                    });
                },
                null);
        }
    }
}
