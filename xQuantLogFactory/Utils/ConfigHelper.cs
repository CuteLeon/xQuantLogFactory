using System;
using System.IO;

namespace xQuantLogFactory.Utils
{
    /// <summary>
    /// 配置助手
    /// </summary>
    public static class ConfigHelper
    {

        /// <summary>
        /// 监视规则XML文件存储目录
        /// </summary>
        public static string MonitorDirectory
        {
            get => Path.Combine(Environment.CurrentDirectory, "Monitor");
        }

        /// <summary>
        /// 日志文件名称格式
        /// </summary>
        /// <remarks>神奇，勿动！</remarks>
        public static string LogFileNameFormat
        {
            get => $@"^.*?({ServerLogFileNamerefix}|{ClientLogFileNamerefix})Log_(Trace|Debug|Info|Warn|Error)\.txt(|\.\d*)$";
        }

        /// <summary>
        /// 服务端日志文件名称前缀
        /// </summary>
        public static string ServerLogFileNamerefix
        {
            get => "Srv";
        }

        /// <summary>
        /// 客户端日志文件名称前缀
        /// </summary>
        public static string ClientLogFileNamerefix
        {
            get => "Clt";
        }

    }
}
