using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Monitor
{
    /// <summary>
    /// 监视规则容器
    /// </summary>
    [Serializable]
    [XmlRoot("MonitorRoot")]
    public class MonitorContainer : MonitorBase, IMonitorContainer
    {
        #region 监视规则列表

        /// <summary>
        /// 客户端和服务端监视规则树根节点
        /// </summary>
        [XmlElement(FixedDatas.TERMINAL_MONITOR_XML_ELEMENT_NAME)]
        public List<TerminalMonitorItem> TerminalMonitorTreeRoots = new List<TerminalMonitorItem>();

        /// <summary>
        /// Performance 监视规则树根节点
        /// </summary>
        [XmlElement(FixedDatas.PERFORMANCE_MONITOR_XML_ELEMENT_NAME)]
        public List<PerformanceMonitorItem> PerformanceMonitorTreeRoots = new List<PerformanceMonitorItem>();
        #endregion

        #region 初始化监视规则树方法

        /// <summary>
        /// 初始化监视规则树
        /// </summary>
        /// <typeparam name="TMonitor"></typeparam>
        /// <typeparam name="TMonitorResult"></typeparam>
        /// <typeparam name="TAnalysisResult"></typeparam>
        /// <typeparam name="TLogFile"></typeparam>
        /// <param name="monitors"></param>
        public void InitMonitorTree<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>(List<TMonitor> monitors)
            where TMonitor : MonitorItemRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>, new()
            where TMonitorResult : MonitorResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TAnalysisResult : AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TLogFile : LogFileRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        {
            if (monitors == null)
            {
                throw new ArgumentNullException(nameof(monitors));
            }

            // 使用一个根节点辅助
            TMonitor root = new TMonitor() { Name = string.Empty, CANO = string.Empty, MonitorTreeRoots = monitors };
            TMonitor currentMonitor = null;

            foreach (TMonitor parentMonitor in root.GetMonitorsWithSelf())
            {
                if (parentMonitor.HasChild())
                {
                    for (int index = 0; index < parentMonitor.MonitorTreeRoots.Count; index++)
                    {
                        // 将父节点的子节点作为当前节点
                        currentMonitor = parentMonitor.MonitorTreeRoots[index];

                        if (string.IsNullOrEmpty(currentMonitor.CANO))
                        {
                            currentMonitor.CANO = parentMonitor.GetNextChildCANO();
                        }

                        // 当监视规则开始条件为空时，使用前驱或外层节点条件填充
                        if (string.IsNullOrEmpty(currentMonitor.StartPattern))
                        {
                            if (index == 0)
                            {
                                // 第一个节点使用父节点的开始条件
                                if (!string.IsNullOrEmpty(parentMonitor.StartPattern))
                                {
                                    currentMonitor.StartPattern = parentMonitor.StartPattern;
                                }
                            }
                            else
                            {
                                // 剩余节点使用前驱节点的结束条件
                                if (!string.IsNullOrEmpty(parentMonitor.MonitorTreeRoots[index - 1].FinishPattern))
                                {
                                    currentMonitor.StartPattern = parentMonitor.MonitorTreeRoots[index - 1].FinishPattern;
                                }
                            }
                        }

                        if (currentMonitor.HasChild())
                        {
                            // 遍历当前节点的子节点
                            currentMonitor.MonitorTreeRoots.ForEach(childMonitor =>
                            {
                                // 复制父级节点配置
                                childMonitor.BindParentMonitor(currentMonitor);
                            });
                        }
                    }
                }
            }

            // 销毁 root
            root = null;
        }

        /// <summary>
        /// 初始化客户端和服务端监视规则树
        /// </summary>
        public void InitTerminalMonitorTree()
        {
            // 先初始化第一批根节点列表
            foreach (var rootMonitor in this.TerminalMonitorTreeRoots)
            {
                // 当父节点不存在表名时，使用默认表名
                if (string.IsNullOrEmpty(rootMonitor.SheetName))
                {
                    rootMonitor.SheetName = FixedDatas.ExcelSourceSheetName;
                }
            }

            this.InitMonitorTree<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>(this.TerminalMonitorTreeRoots);
        }

        /// <summary>
        /// 初始化Performance监视规则树
        /// </summary>
        public void InitPerformanceMonitorTree()
        {
            this.InitMonitorTree<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>(this.PerformanceMonitorTreeRoots);
        }
        #endregion

        #region 扫描监视规则方法

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
        /// 扫描监视规则树
        /// </summary>
        /// <typeparam name="TMonitor"></typeparam>
        /// <typeparam name="TMonitorResult"></typeparam>
        /// <typeparam name="TAnalysisResult"></typeparam>
        /// <typeparam name="TLogFile"></typeparam>
        /// <param name="monitors"></param>
        /// <returns></returns>
        public IEnumerable<TMonitor> GetMonitorTree<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>(List<TMonitor> monitors)
            where TMonitor : MonitorItemRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>, new()
            where TMonitorResult : MonitorResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TAnalysisResult : AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TLogFile : LogFileRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        {
            if (monitors == null)
            {
                throw new ArgumentNullException(nameof(monitors));
            }

            TMonitor root = new TMonitor() { Name = string.Empty, CANO = string.Empty, MonitorTreeRoots = monitors };

            foreach (var monitor in root.GetMonitors())
            {
                yield return monitor;
            }

            // 销毁 root
            root = null;
        }

        /// <summary>
        /// 扫描容器所有客户端和服务端监视规则节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TerminalMonitorItem> GetTerminalMonitorItems()
        {
            return this.GetMonitorTree<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>(this.TerminalMonitorTreeRoots);
        }

        /// <summary>
        /// 扫描容器所有Performance监视规则节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PerformanceMonitorItem> GetPerformanceMonitorItems()
        {
            return this.GetMonitorTree<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>(this.PerformanceMonitorTreeRoots);
        }
        #endregion
    }
}
