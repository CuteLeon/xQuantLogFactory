using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using xQuantLogFactory.Model;
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

            //使用内嵌资源当做默认监视规则
            if (files.Length == 0)
            {
                try
                {
                    UnityResource.Monitor_Template.SaveToFile(ConfigHelper.DefaultMonitorXMLPath);
                }
                catch { throw; }

                return new string[] { ConfigHelper.DefaultMonitorXMLPath };
            }

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
            //存在子级监视规则等待遍历的父级规则
            Queue<IMonitor> parentItems = new Queue<IMonitor>();

            //创建所有容器对象
            foreach (string xmlFile in this.GetChildFiles(directory, file => file.EndsWith(".XML", StringComparison.OrdinalIgnoreCase)))
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
                //不约束监视规则时使用全部监视规则
                if (argument.MonitorItemNames.Count == 0)
                {
                    targetItems.AddRange(CurrentMonitor.MonitorItems);
                    //不需要遍历子规则
                    continue;
                }
                else
                {
                    //查询匹配的监视规则对象并一并关联子对象
                    targetItems.AddRange(
                            from monitor in CurrentMonitor.MonitorItems
                            where argument.MonitorItemNames.Contains(monitor.Name)
                            select monitor
                            );
                }

                //不符合的父级规则需要入队继续深度遍历
                foreach (IMonitor monitor in CurrentMonitor.MonitorItems
                    .Where(monitor => !argument.MonitorItemNames.Contains(monitor.Name) && monitor.HasChildren)
                    )
                {
                    parentItems.Enqueue(monitor);
                }
            }

            return targetItems as IEnumerable<T>;
        }

    }
}
