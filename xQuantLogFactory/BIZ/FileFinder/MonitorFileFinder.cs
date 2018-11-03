using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using xQuantLogFactory.Utils.Extensions;
using xQuantLogFactory.Model;

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
        /// <returns></returns>
        public IEnumerable<string> GetChildFiles(string directory, Predicate<string> predicate = null)
        {
            return predicate == null ? Directory.GetFiles(directory) : Array.FindAll(Directory.GetFiles(directory), predicate);
        }

        /// <summary>
        /// 将目录内XML文件反序列化为监视规则容器对象
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="argument">任务参数</param>
        /// <returns></returns>
        public IEnumerable<T> GetFiles<T>(string directory, TaskArgument argument) where T : class
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //符合要求的监视规则
            List<MonitorItem> targetItems = new List<MonitorItem>();
            //存在子级监视规则等待遍历的父级规则
            Queue<IMonitor> parentItems = new Queue<IMonitor>();

            //创建所有容器对象
            foreach (string xmlFile in this.GetChildFiles(directory, file => file.ToUpper().EndsWith(".XML")))
            {
                //维护监视规则容器列表
                MonitorContainer container = File.ReadAllText(xmlFile, Encoding.UTF8).DeserializeToObject<MonitorContainer>();
                if (container != null && container.HasChildren)
                {
                    //容器入队
                    parentItems.Enqueue(container);
                }
            }

            //循环指针
            IMonitor CurrentMonitor = null;
            while (parentItems.Count > 0)
            {
                //首元素出队
                CurrentMonitor = parentItems.Dequeue();

                //查询匹配的监视规则对象 (返回的IMonitor对象清空子级监视规则对象，以免被ORM重复记录到数据库)
                targetItems.AddRange(
                        from monitor in CurrentMonitor.MonitorItems
                        where argument.ItemNames.Contains(monitor.Name)
                        select monitor.Clone() as MonitorItem
                        );

                //新的父元素依然入队
                foreach (IMonitor monitor in CurrentMonitor.MonitorItems)
                    if (monitor.HasChildren)
                        parentItems.Enqueue(monitor);
            }

            return targetItems as IEnumerable<T>;
        }

    }
}
