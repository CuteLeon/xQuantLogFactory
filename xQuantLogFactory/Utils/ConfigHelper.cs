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
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Monitor");
        }

        /// <summary>
        /// 日志报告导出目录
        /// </summary>
        public static string ReportExportDirectory
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Export");
        }

        /// <summary>
        /// 日志文件名称格式
        /// </summary>
        /// <remarks>神奇，勿动！</remarks>
        public static string LogFileNameFormat
        {
            get => $@"^(({ServerLogFileNamePrefix}|{ClientLogFileNamePrefix})Log_(Trace|Debug|Info|Warn|Error)\.txt(|\.\d*))|(performanceLog\d{{8}}\.txt)$";
        }

        /// <summary>
        /// 服务端日志文件名称前缀
        /// </summary>
        public static string ServerLogFileNamePrefix
        {
            get => "Srv";
        }

        /// <summary>
        /// 客户端日志文件名称前缀
        /// </summary>
        public static string ClientLogFileNamePrefix
        {
            get => "Clt";
        }

        /// <summary>
        /// 中间件日志文件名称前缀
        /// </summary>
        public static string MiddlewareLogFileNamePrefix
        {
            get => "performanceLog";
        }

    }
}
