using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Extensions;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 监视规则容器文件查找器
    /// </summary>
    public class MonitorFileFinder : IChildFileFinder, ITaskFileFinder
    {
        /// <summary>
        /// 在指定目录查找复核要求的文件
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="predicate">文件筛选条件</param>
        /// <param name="searchPattern">匹配模式</param>
        /// <returns></returns>
        public IEnumerable<string> GetChildFiles(string directory, Predicate<string> predicate = null, string searchPattern = "*")
        {
            string[] files = Directory.GetFiles(directory, searchPattern);
            if (predicate != null) files = Array.FindAll(files, predicate);

            return files;
        }

        /// <summary>
        /// 将目录内XML文件反序列化为监视规则容器对象
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="argument">任务参数</param>
        /// <returns></returns>
        /// <remarks>EF6会自动关联 MonitorItem 并将子节点一并入库并与父节点关联ID</remarks>
        public IEnumerable<T> GetFiles<T>(string directory, TaskArgument argument) where T : class
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //符合要求的监视规则
            List<MonitorItem> targetItems = new List<MonitorItem>();

            //创建所有容器对象
            foreach (string xmlFile in this.GetChildFiles(directory,
                file => file.EndsWith(argument.MonitorFileName, StringComparison.OrdinalIgnoreCase)
                ))
            {
                //维护监视规则容器列表
                MonitorContainer container = File.ReadAllText(xmlFile, Encoding.UTF8).DeserializeToObject<MonitorContainer>();
                if (container != null && container.HasChildren)
                {
                    //记录监视规则容器的根节点列表
                    targetItems.AddRange(container.MonitorTreeRoots);
                }
            }

            return targetItems as IEnumerable<T>;
        }

    }
}
