using System;
using System.Collections.Generic;
using System.IO;

using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 任务相关文件查找器基类
    /// </summary>
    public abstract class TaskFileFinderBase
    {
        /// <summary>
        /// 在指定目录查找符合要求的文件
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="predicate">文件筛选条件</param>
        /// <param name="searchPattern">匹配模式</param>
        /// <returns></returns>
        public IEnumerable<string> GetChildFiles(string directory, Predicate<string> predicate = null, string searchPattern = "*")
        {
            string[] files = Directory.GetFiles(directory, searchPattern);
            if (predicate != null)
            {
                files = Array.FindAll(files, predicate);
            }

            return files;
        }

        /// <summary>
        /// 查找符合日志分析参数的文件转义对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="directory">文件存放目录</param>
        /// <param name="argument">任务参数</param>
        /// <returns></returns>
        public abstract T GetTaskObject<T>(string directory, TaskArgument argument)
            where T : class;
    }
}
