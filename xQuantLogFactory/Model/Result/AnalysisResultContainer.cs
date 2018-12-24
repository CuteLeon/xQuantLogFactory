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
        /// <param name="targetAnalysisResults">目标列表</param>
        public void InitAnalysisResultTree<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>(IEnumerable<TAnalysisResult> sourceAnalysisResults, List<TAnalysisResult> targetAnalysisResults)
            where TMonitor : MonitorItemBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>, new()
            where TMonitorResult : MonitorResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TAnalysisResult : AnalysisResultBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
            where TLogFile : LogFileBase<TMonitor, TMonitorResult, TAnalysisResult, TLogFile>
        {
            if (sourceAnalysisResults == null)
            {
                throw new System.ArgumentNullException(nameof(sourceAnalysisResults));
            }

            // 清理现有关系树
            targetAnalysisResults.Clear();
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
        }

        /// <summary>
        /// 初始化客户端和服务端分析结果树
        /// </summary>
        /// <param name="analysisResults"></param>
        public void InitTerminalAnalysisResultTree(IEnumerable<TerminalAnalysisResult> analysisResults)
        {
            this.InitAnalysisResultTree<TerminalMonitorItem, TerminalMonitorResult, TerminalAnalysisResult, TerminalLogFile>(analysisResults, this.TerminalAnalysisResultRoots);
        }

        /// <summary>
        /// 初始化Performance分析结果树
        /// </summary>
        /// <param name="analysisResults"></param>
        public void InitPerformanceAnalysisResultTree(List<PerformanceAnalysisResult> analysisResults)
        {
            this.InitAnalysisResultTree<PerformanceMonitorItem, PerformanceMonitorResult, PerformanceAnalysisResult, PerformanceLogFile>(analysisResults, this.PerformanceAnalysisResultRoots);
        }
        #endregion
    }
}
