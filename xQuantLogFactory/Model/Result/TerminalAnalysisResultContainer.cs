using System.Collections.Generic;
using System.Linq;

using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果容器
    /// </summary>
    public class TerminalAnalysisResultContainer
    {
        /// <summary>
        /// Gets or sets 子分析结果
        /// </summary>
        public List<TerminalAnalysisResult> AnalysisResultRoots { get; set; } = new List<TerminalAnalysisResult>();

        /// <summary>
        /// 初始化分析结果树
        /// </summary>
        /// <param name="analysisResults"></param>
        public void InitAnalysisResultTree(IEnumerable<TerminalAnalysisResult> analysisResults)
        {
            // 清理现有关系树
            this.AnalysisResultRoots.Clear();
            foreach (var analysisResult in analysisResults)
            {
                analysisResult.ParentAnalysisResult = null;
                analysisResult.AnalysisResultRoots.Clear();
            }

            foreach (TerminalAnalysisResult analysisResult in analysisResults)
            {
                // 判断分析结果是否为完整组
                if (analysisResult.IsIntactGroup())
                {
                    if (analysisResult.MonitorItem.HasChild())
                    {
                        // 关联子监视规则的分析结果（以当前节点为树根遍历监视规则树，防止分析结果之间断级，而造成分析结果树断开）
                        foreach (TerminalMonitorItem childMonitor in analysisResult.MonitorItem.GetMonitors())
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
                        this.AnalysisResultRoots.Add(analysisResult);
                    }
                }
            }

            // 根据分析结果父级节点反向关联子分析结果，单独处理，防止重复高频操作
            foreach (var analysisResult in analysisResults)
            {
                if (analysisResult.ParentAnalysisResult != null)
                {
                    analysisResult.ParentAnalysisResult.AnalysisResultRoots.Add(analysisResult);
                }
            }
        }
    }
}
