using System;
using System.IO;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 配置助手
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 监视规则XML文件存储目录
        /// </summary>
        public string MonitorDirectory
        {
            get => Path.Combine(Environment.CurrentDirectory, "Monitor");
        }
    }
}
