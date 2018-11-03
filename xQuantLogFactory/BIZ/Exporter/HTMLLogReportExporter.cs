using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Exporter
{
    /// <summary>
    /// HTML日志报告导出器
    /// </summary>
    public class HTMLLogReportExporter : LogProcesserBase, ILogReportExporter
    {
        /// <summary>
        /// 导出日志报告
        /// </summary>
        /// <param name="reportPath">报告文件路径</param>
        /// <param name="argument">任务参数</param>
        public void ExportReport(string reportPath, TaskArgument argument)
        {
            //TODO: HTML 报告导出逻辑
        }
    }
}
