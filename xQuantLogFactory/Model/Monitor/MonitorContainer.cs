using System;
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
            MonitorItem currentMonitor = null;

            // 处理所有父节点
            foreach (MonitorBase parentMonitor in this.GetMonitorBases())
            {
                if (parentMonitor.HasChildren)
                {
                    // 处理父节点的子节点
                    for (int index = 0; index < parentMonitor.MonitorTreeRoots.Count; index++)
                    {
                        // 将父节点的子节点作为当前节点
                        currentMonitor = parentMonitor.MonitorTreeRoots[index];

                        // 预设默认表名
                        if (string.IsNullOrEmpty(currentMonitor.SheetName))
                        {
                            currentMonitor.SheetName = ConfigHelper.ExcelSourceSheetName;
                        }

                        // 当监视规则开始条件为空时，使用前驱或外层节点条件填充
                        if (string.IsNullOrEmpty(currentMonitor.StartPattern))
                        {
                            if (index == 0)
                            {
                                // 容器的第一层根节点无法取容器的开始条件，因为容器没有监视条件
                                if (parentMonitor is MonitorItem parent)
                                {
                                    // 第一个节点使用父节点的开始条件
                                    if (!string.IsNullOrEmpty(parent.StartPattern))
                                    {
                                        currentMonitor.StartPattern = parent.StartPattern;
                                    }
                                }
                            }
                            else
                            {
                                // 剩余节点使用前驱节点的结束条件
                                if (!string.IsNullOrEmpty(parentMonitor.MonitorTreeRoots[index - 1].FinishPatterny))
                                {
                                    currentMonitor.StartPattern = parentMonitor.MonitorTreeRoots[index - 1].FinishPatterny;
                                }
                            }
                        }

                        if (currentMonitor.HasChildren)
                        {
                            // 遍历当前节点的子节点
                            currentMonitor.MonitorTreeRoots.ForEach(childMonitor =>
                            {
                                // 复制当前节点表名给子节点
                                if (string.IsNullOrEmpty(childMonitor.SheetName))
                                {
                                    childMonitor.SheetName = currentMonitor.SheetName;
                                }

                                // 复制父级节点配置
                                childMonitor.BindParentMonitor(currentMonitor);
                            });
                        }
                    }
                }
            }
        }
    }
}
