using System.Collections.Generic;

using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model.EqualityComparer
{
    /// <summary>
    /// 监视结果比较器
    /// </summary>
    /// <remarks>可能同一条日志内容被多个监视规则重复命中而产生多条结果记录，此比较器为了打击山寨</remarks>
    public class LogResultEqualityComparer<T> : IEqualityComparer<T>
        where T : LogResultBase
    {
        public bool Equals(T x, T y)
        {
            return
                string.Equals(
                    x.LogFile?.FilePath,
                    y.LogFile?.FilePath,
                    System.StringComparison.OrdinalIgnoreCase) &&
                x.LineNumber == y.LineNumber;
        }

        public int GetHashCode(T obj)
            => obj.GetHashCode();
    }
}
