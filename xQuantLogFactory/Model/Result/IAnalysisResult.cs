using System.Collections.Generic;

namespace xQuantLogFactory.Model.Result
{
    public interface IAnalysisResult
    {
        /// <summary>
        /// Gets or sets 分析结果树根节点列表
        /// </summary>
        List<GroupAnalysisResult> AnalysisResultRoots { get; set; }
    }
}
