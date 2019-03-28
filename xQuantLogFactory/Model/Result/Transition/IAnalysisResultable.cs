namespace xQuantLogFactory.Model.Result.Transition
{
    /// <summary>
    /// 可转储到分析结果接口
    /// </summary>
    /// <typeparam name="TAnalysisResult"></typeparam>
    public interface IAnalysisResultable<TAnalysisResult>
        where TAnalysisResult : IAnalysisResult
    {
        /// <summary>
        /// 转储到分析结果
        /// </summary>
        /// <param name="analysisResult"></param>
        /// <returns></returns>
        TAnalysisResult ToAnalysisResult(ref TAnalysisResult analysisResult);
    }
}
