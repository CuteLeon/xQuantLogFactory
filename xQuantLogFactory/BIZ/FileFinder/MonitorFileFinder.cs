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
        /// <returns></returns>
        public IEnumerable<T> GetFiles<T>(TaskArgument argumant) where T : class
        {
            if (argumant == null)
                throw new ArgumentNullException(nameof(argumant));
            if (!Directory.Exists(argumant.BaseDirectory))
                throw new DirectoryNotFoundException(argumant.BaseDirectory);

            //MonitorItem 为树状结构，但当监视父级节点时，不主动监视未声明的子级节点
            List<MonitorItem> monitorItems = new List<MonitorItem>();
            foreach (string xmlFile in this.GetChildFiles(argumant.BaseDirectory, file => file.ToUpper().EndsWith(".XML")))
            {
                MonitorContainer container = File.ReadAllText(xmlFile, Encoding.UTF8).DeserializeToObject<MonitorContainer>();
                //TODO: 需要筛选子项
                monitorItems.AddRange(
                    from monitor in container.ItemList where argumant.ItemNames.Contains(monitor.Name) select monitor
                    );
            }
            return monitorItems as IEnumerable<T>;
        }
    }
}
