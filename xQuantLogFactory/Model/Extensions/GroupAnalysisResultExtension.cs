using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.Extensions
{
    /// <summary>
    /// 组分析结果扩展
    /// </summary>
    public static class GroupAnalysisResultExtension
    {

        /// <summary>
        /// 是否为完整组
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IsIntactGroup(this GroupAnalysisResult result)
        {
            if (result == null) return false;

            return
                result.StartMonitorResult != null &&
                result.FinishMonitorResult != null;
        }

        /// <summary>
        /// 获取优先监视结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static MonitorResult FirstResultOrDefault(this GroupAnalysisResult result)
        {
            if (result == null) return null;

            return result.StartMonitorResult ?? result.FinishMonitorResult;
        }

    }
}
