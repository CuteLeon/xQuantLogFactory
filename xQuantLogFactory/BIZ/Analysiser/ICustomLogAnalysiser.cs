using System.Text.RegularExpressions;

namespace xQuantLogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 自定义分析器
    /// </summary>
    public interface ICustomLogAnalysiser
    {
        /// <summary>
        /// Gets gets or sets 分析器正则表达式
        /// </summary>
        Regex AnalysisRegex { get; }
    }
}
