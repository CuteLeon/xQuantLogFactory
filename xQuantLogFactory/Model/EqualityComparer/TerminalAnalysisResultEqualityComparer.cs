using System.Collections.Generic;

using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.EqualityComparer
{
    /// <summary>
    /// 客户端和服务端监视结果比较器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>可能同一条日志内容被多个监视规则重复命中而产生多条结果记录，此比较器为了打击山寨</remarks>
    public class TerminalAnalysisResultEqualityComparer<T> : IEqualityComparer<T>
        where T : TerminalAnalysisResult
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            return
                string.Equals(
                    x.LogFile?.FilePath,
                    y.LogFile?.FilePath,
                    System.StringComparison.OrdinalIgnoreCase) &&
                x.LineNumber == y.LineNumber;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
            => obj.GetHashCode();
    }
}
