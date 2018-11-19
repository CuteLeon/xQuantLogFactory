using System;
using System.IO;
using xQuantLogFactory.Model;

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
        /// 默认监视规则XML文件路径
        /// </summary>
        public static string DefaultMonitorXMLPath
        {
            get => Path.Combine(MonitorDirectory, "Monitor.xml");
        }

        /// <summary>
        /// 日志文件名称格式
        /// </summary>
        /// <remarks>神奇，勿动！</remarks>
        public static string LogFileNameFormat
        {
            get => $@"^(({ServerLogFileNamePrefix}|{ClientLogFileNamePrefix})Log_{LogFileLevel}\.txt(|\.\d*))|(performanceLog\d{{8}}\.txt)$";
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
        /// 日志文件等级
        /// </summary>
        public static string LogFileLevel { get; set; } = "DEBUG";

        /// <summary>
        /// 中间件日志文件名称前缀
        /// </summary>
        public static string MiddlewareLogFileNamePrefix
        {
            get => "performanceLog";
        }

        /// <summary>
        /// 默认导出报告模式
        /// </summary>
        public static ReportModes DefaultReportMode
        {
            get => ReportModes.HTML;
        }

        /// <summary>
        /// 是否使用GUI任务工厂
        /// </summary>
        public static bool UseGUITaskFactory
        {
            get => Environment.GetCommandLineArgs().Length <= 1;
        }

    }
}
