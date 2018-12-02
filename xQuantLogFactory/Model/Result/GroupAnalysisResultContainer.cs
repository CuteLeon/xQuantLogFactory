using System.Collections.Generic;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 分析结果容器
    /// </summary>
    public class GroupAnalysisResultContainer : IAnalysisResult
    {
        /// <summary>
        /// Gets or sets 子分析结果
        /// </summary>
        public List<GroupAnalysisResult> AnalysisResultRoots { get; set; } = new List<GroupAnalysisResult>();
    }
}
