﻿using System;
using System.IO;
using System.Text;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils.Extensions;

namespace xQuantLogFactory.BIZ.FileFinder
{
    /// <summary>
    /// 监视规则容器文件查找器
    /// </summary>
    public class MonitorFileFinder : TaskFileFinderBase
    {
        /// <summary>
        /// 将目录内XML文件反序列化为监视规则容器对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory">文件目录</param>
        /// <param name="argument">任务参数</param>
        /// <returns></returns>
        /// <remarks>EF6会自动关联 MonitorItem 并将子节点一并入库并与父节点关联ID</remarks>
        public override T GetTaskObject<T>(string directory, TaskArgument argument)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(nameof(directory));
            }

            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 创建所有容器对象
            foreach (string xmlFile in this.GetChildFiles(
                directory,
                file => file.EndsWith(argument.MonitorFileName, StringComparison.OrdinalIgnoreCase)))
            {
                MonitorContainer container = File.ReadAllText(xmlFile, Encoding.UTF8).DeserializeToObject<MonitorContainer>();
                container?.InitMonitorSheetName();

                return container as T;
            }

            return null;
        }
    }
}
