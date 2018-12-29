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
        /// Gets 监视规则XML文件存储目录
        /// </summary>
        public static string MonitorDirectory
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Monitor");
        }

        /// <summary>
        /// Gets 日志报告导出目录
        /// </summary>
        public static string ReportExportDirectory
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Export");
        }

        /// <summary>
        /// Gets 报告导出模板目录
        /// </summary>
        public static string ReportTempletDirectory
        {
            get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportTemplet");
        }

        /// <summary>
        /// Gets excel报告导出模板文件名称
        /// </summary>
        public static string ExcelReportTempletPath
        {
            get => Path.Combine(ReportTempletDirectory, "xQuant_EXCEL_Templet.xlsx");
        }

        /// <summary>
        /// Gets a value indicating whether 是否使用GUI任务工厂
        /// </summary>
        public static bool UseGUITaskFactory
        {
            get => Environment.GetCommandLineArgs().Length <= 1;
        }
    }
}
