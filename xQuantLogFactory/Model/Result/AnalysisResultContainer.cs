using System;
using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果容器
    /// </summary>
    public class AnalysisResultContainer
    {
        #region 数据集合

        /// <summary>
        /// Gets or sets 客户端和服务端分析结果树根节点列表
        /// </summary>
        public List<TerminalAnalysisResult> TerminalAnalysisResultRoots { get; set; } = new List<TerminalAnalysisResult>();

        /// <summary>
        /// Gets or sets Performance分析结果树根节点列表
        /// </summary>
        public List<PerformanceAnalysisResult> PerformanceAnalysisResultRoots { get; set; } = new List<PerformanceAnalysisResult>();
        #endregion

        #region 初始化分析结果树

        /// <summary>
        /// 初始化分析结果树
        /// </summary>
        /// <typeparam name="TMonitor"></typeparam>
        /// <typeparam name="TMonitorResult"></typeparam>
        /// <typeparam name="TAnalysisResult"></typeparam>
        /// <typeparam name="TLogFile"></typeparam>
        /// <param name="sourceAnalysisResults">源数据</param>
        /// <returns>树根节点列表</returns>
        public List<TAnalysisResult> InitAnalysisResultTree<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>(IEnumerable<TAnalysisResult> sourceAnalysisResults)
            where TMonitor : MonitorItemRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>, new()
            where TMonitorResult : MonitorResultRelBase<TMonitor, TMonitorResult,  TLogFile>
            where TAnalysisResult : AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TLogFile : LogFileBase
        {
            if (sourceAnalysisResults == null)
            {
                throw new ArgumentNullException(nameof(sourceAnalysisResults));
            }

            List<TAnalysisResult> targetAnalysisResults = new List<TAnalysisResult>();

            // 清理现有关系
            foreach (var analysisResult in sourceAnalysisResults)
            {
                analysisResult.ParentAnalysisResult = null;
                analysisResult.AnalysisResultRoots.Clear();
            }

            foreach (TAnalysisResult analysisResult in sourceAnalysisResults)
            {
                // 判断分析结果是否为完整组
                if (analysisResult.IsIntactGroup())
                {
                    if (analysisResult.MonitorItem.HasChild())
                    {
                        // 关联子监视规则的分析结果（以当前节点为树根遍历监视规则树，防止分析结果之间断级，而造成分析结果树断开）
                        foreach (TMonitor childMonitor in analysisResult.MonitorItem.GetMonitors())
                        {
                            childMonitor.AnalysisResults
                                .Where(result =>
                                    analysisResult.StartMonitorResult.LogTime <= result.LogTime &&
                                    result.LogTime <= analysisResult.FinishMonitorResult.LogTime)
                                .ToList().ForEach(childResult =>
                                {
                                    // 节约性能，仅关联父节点，防止重复高频的List操作
                                    childResult.ParentAnalysisResult = analysisResult;
                                });
                        }
                    }

                    // 【不存在父级】的【完整】分析组节点记录为分析结果容器的根节点，避免大量错误的不完整组结果污染报告
                    if (analysisResult.ParentAnalysisResult == null)
                    {
                        targetAnalysisResults.Add(analysisResult);
                    }
                }
            }

            // 根据分析结果父级节点反向关联子分析结果，单独处理，防止重复高频操作
            foreach (var analysisResult in sourceAnalysisResults)
            {
                if (analysisResult.ParentAnalysisResult != null)
                {
                    analysisResult.ParentAnalysisResult.AnalysisResultRoots.Add(analysisResult);
                }
            }

            return targetAnalysisResults;
        }

        /// <summary>
        /// 初始化客户端和服务端分析结果树
        /// </summary>
        /// <param name="analysisResults"></param>
        public void InitTerminalAnalysisResultTree(IEnumerable<TerminalAnalysisResult> analysisResults)
        {
            // 清理现有分析结果树根
            this.TerminalAnalysisResultRoots.Clear();

            this.TerminalAnalysisResultRoots.AddRange(
                this.InitAnalysisResultTree<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>(analysisResults));
        }

        /// <summary>
        /// 初始化Performance分析结果树
        /// </summary>
        /// <param name="analysisResults"></param>
        public void InitPerformanceAnalysisResultTree(List<PerformanceAnalysisResult> analysisResults)
        {
            // 清理现有分析结果树根
            this.PerformanceAnalysisResultRoots.Clear();

            // 以IP和用户代码对分析结果分组后再构建分析结果树
            foreach (var resultGroup in analysisResults.GroupBy(result => (result.IPAddress, result.UserCode)))
            {
                this.PerformanceAnalysisResultRoots.AddRange(
                    this.InitAnalysisResultTree<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>(resultGroup));
            }
        }
        #endregion

        #region 扫描分析结果树

        /// <summary>
        /// 是否含有客户端和服务端分析结果
        /// </summary>
        /// <returns></returns>
        public bool HasTerminalAnalysisResults()
        {
            return this.TerminalAnalysisResultRoots != null && this.TerminalAnalysisResultRoots.Count > 0;
        }

        /// <summary>
        /// 是否含有Performance分析结果
        /// </summary>
        /// <returns></returns>
        public bool HasPerformanceAnalysisResults()
        {
            return this.PerformanceAnalysisResultRoots != null && this.PerformanceAnalysisResultRoots.Count > 0;
        }

        /// <summary>
        /// 获取所有子分析结果及其子分析结果
        /// </summary>
        /// <typeparam name="TMonitor"></typeparam>
        /// <typeparam name="TMonitorResult"></typeparam>
        /// <typeparam name="TAnalysisResult"></typeparam>
        /// <typeparam name="TLogFile"></typeparam>
        /// <param name="analysisResults"></param>
        /// <returns></returns>
        /// <remarks>IEnumerable<>对象即使储存为变量，每次访问依然会进入此方法，若要减少计算量，需要将此方法返回数据 .ToList()</remarks>
        public IEnumerable<TAnalysisResult> GetAnalysisResults<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>(List<TAnalysisResult> analysisResults)
            where TMonitor : MonitorItemRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TMonitorResult : MonitorResultRelBase<TMonitor, TMonitorResult, TLogFile>
            where TAnalysisResult : AnalysisResultRelBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>, new()
            where TLogFile : LogFileBase
        {
            if (analysisResults == null)
            {
                throw new ArgumentNullException(nameof(analysisResults));
            }

            TAnalysisResult root = new TAnalysisResult() { AnalysisResultRoots = analysisResults };

            foreach (var analysisResult in root.GetAnalysisResults())
            {
                yield return analysisResult;
            }

            // 销毁 root
            root = null;
        }

        /// <summary>
        /// 获取客户端和服务端分析结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TerminalAnalysisResult> GetTerminalAnalysisResults()
        {
            return this.GetAnalysisResults<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>(this.TerminalAnalysisResultRoots);
        }

        /// <summary>
        /// 获取Performance分析结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PerformanceAnalysisResult> GetPerformanceAnalysisResults()
        {
            return this.GetAnalysisResults<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>(this.PerformanceAnalysisResultRoots);
        }
        #endregion
    }
}
