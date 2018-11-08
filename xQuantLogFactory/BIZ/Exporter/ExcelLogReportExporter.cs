using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils.Extensions;

namespace xQuantLogFactory.BIZ.Exporter
{
    /// <summary>
    /// Excel 表格导出器
    /// </summary>
    public class ExcelLogReportExporter : LogProcesserBase, ILogReportExporter
    {

        /// <summary>
        /// 导出Excel报告
        /// </summary>
        /// <param name="reportPath">报告路径</param>
        /// <param name="argument">任务参数</param>
        public void ExportReport(string reportPath, TaskArgument argument)
        {
            //导出模板
            try
            {
                UnityResource.xQuant_EXCEL_Templet.SaveToFile(reportPath);
            }
            catch { throw; }


        }

    }
}
