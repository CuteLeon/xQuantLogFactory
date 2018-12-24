using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("MonitorRoot")]
    public class MonitorContainer : MonitorBase, IMonitorContainer
    {
        /// <summary>
        /// 客户端和服务端监视规则树根节点
        /// </summary>
        List<TerminalMonitorItem> TerminalMonitorTreeRoots = new List<TerminalMonitorItem>();

        /// <summary>
        /// Performance 监视规则树根节点
        /// </summary>
        List<PerformanceMonitorItem> PerformanceMonitorTreeRoots = new List<PerformanceMonitorItem>();

        /// <summary>
        /// 是否含有客户端和服务端监视规则
        /// </summary>
        /// <returns></returns>
        public bool HasTerminalMonitors() => this.TerminalMonitorTreeRoots != null && this.TerminalMonitorTreeRoots.Count > 0;

        /// <summary>
        /// 是否含有Performance监视规则
        /// </summary>
        /// <returns></returns>
        public bool HasPerformanceMonitors() => this.PerformanceMonitorTreeRoots != null && this.PerformanceMonitorTreeRoots.Count > 0;

        /// <summary>
        /// 初始化监视规则树
        /// </summary>
        public void InitTerminalMonitorTree()
        {
            // TODO: 初始化监视规则树
            /*
            TerminalMonitorItem currentMonitor = null;

            // 处理所有父节点
            foreach (MonitorBase parentMonitor in this.GetMonitorBasesWithSelf())
            {
                if (parentMonitor.HasChildren)
                {
                    // 处理父节点的子节点
                    for (int index = 0; index < parentMonitor..Count; index++)
                    {
                        // 将父节点的子节点作为当前节点
                        currentMonitor = parentMonitor.TerminalMonitorTreeRoots[index];

                        // 预设默认表名
                        if (string.IsNullOrEmpty(currentMonitor.SheetName))
                        {
                            currentMonitor.SheetName = ConfigHelper.ExcelSourceSheetName;
                        }

                        if (string.IsNullOrEmpty(currentMonitor.CANO))
                        {
                            currentMonitor.CANO = parentMonitor.GetNextChildCANO();
                        }

                        // 当监视规则开始条件为空时，使用前驱或外层节点条件填充
                        if (string.IsNullOrEmpty(currentMonitor.StartPattern))
                        {
                            if (index == 0)
                            {
                                // 容器的第一层根节点无法取容器的开始条件，因为容器没有监视条件
                                if (parentMonitor is TerminalMonitorItem parent)
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

                        if (currentMonitor.HasChild)
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
             */
        }

        /// <summary>
        /// 扫描容器子节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TerminalMonitorItem> GetTerminalMonitorItems()
        {
            // TODO: 实现容器扫描
            return null;
        }

        /// <summary>
        /// 初始化监视规则树
        /// </summary>
        public void InitPerformanceMonitorTree()
        {
            // TODO: 初始化监视规则树
        }

        /// <summary>
        /// 扫描容器子节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PerformanceMonitorItem> GetPerformanceMonitorItems()
        {
            // TODO: 实现容器扫描
            return null;
        }
    }
}
