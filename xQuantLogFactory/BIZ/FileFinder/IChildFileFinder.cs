using System;
using System.Collections.Generic;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 子文件查找器
    /// </summary>
    public interface IChildFileFinder
    {
        /// <summary>
        /// 在指定目录查找复核要求的文件
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="predicate">文件筛选条件</param>
        /// <param name="searchPattern">匹配模式</param>
        /// <returns></returns>
        IEnumerable<string> GetChildFiles(string directory, Predicate<string> predicate = null, string searchPattern = null);

    }
}
