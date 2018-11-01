using System;
using System.IO;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 配置助手
    /// </summary>
    public class ConfigHelper
    {
        //TODO: 此类改为静态类，使用静态字段公开配置信息

        /// <summary>
        /// 监视规则XML文件存储目录
        /// </summary>
        public string MonitorDirectory
        {
            get => Path.Combine(Environment.CurrentDirectory, "Monitor");
        }

        //TODO: 使用常量记录客户端和服务端日志开头字符

    }
}
