using System.Collections.Generic;

using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 任务相关文件查找接口
    /// </summary>
    public interface ITaskFileFinder
    {

        /// <summary>
        /// 查找符合日志分析参数的文件清单
        /// </summary>
        /// <typeparam name="T">转义对象类型</typeparam>
        /// <param name="directory">文件存放目录</param>
        /// <param name="argument">任务参数</param>
        /// <returns>文件清单</returns>
        IEnumerable<T> GetFiles<T>(string directory, TaskArgument argument) where T : class;

    }
}
