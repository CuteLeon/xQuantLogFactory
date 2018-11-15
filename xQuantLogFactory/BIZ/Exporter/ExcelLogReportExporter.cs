using System;
using System.IO;
using System.Linq;

using OfficeOpenXml;

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
            //TODO: 无法导出数据库内已有任务的数据

            //导出模板
            try
            {
                UnityResource.xQuant_EXCEL_Templet.SaveToFile(reportPath);
            }
            catch { throw; }

            //连接Excel文件
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(reportPath)))
            {
                try
                {
                    OfficeProperties properties = excel.Workbook.Properties;
                    properties.Author = "xQuant日志分析工具";
                    properties.Category = "xQuant日志分析报告";
                    properties.Comments = $"xQuant日志分析报告-{argument.TaskID}";
                    properties.Company = "xQuant";
                    properties.Created = DateTime.Now;
                    properties.Manager = "xQuant日志分析工具";
                    properties.Subject = $"xQuant日志分析报告-{argument.TaskID}";
                    properties.Title = $"xQuant日志分析报告-{argument.TaskID}";

                    ExcelWorksheet sourceDataSheet = excel.Workbook.Worksheets["原始"];
                    ExcelWorksheet analysisSheet = excel.Workbook.Worksheets["分析"];

                    //数据区域从 [2, 1] 开始
                    using (ExcelRange range = sourceDataSheet.Cells[2, 1, argument.AnalysisResults.Count + 1, 8])
                    {
                        int rowID = 2, executeID = 0;
                        foreach (var result in argument.AnalysisResults
                            .Where(result => result.StartMonitorResult != null && result.FinishMonitorResult != null)
                            .OrderBy(result => (result.LogFile?.FileID, result.LineNumber))
                            )
                        {
                            if (result.MonitorItem?.ParentMonitorItem == null) executeID++;

                            range[rowID, 1].Value = result.MonitorItem?.Name;
                            range[rowID, 2].Value = result.MonitorItem?.ParentMonitorItem?.Name;
                            range[rowID, 3].Value = result.Version;
                            range[rowID, 4].Value = executeID;
                            range[rowID, 5].Value = result.ElapsedMillisecond;
                            range[rowID, 6].Value = result.StartMonitorResult?.LogContent;
                            range[rowID, 7].Value = result.FinishMonitorResult?.LogContent;
                            range[rowID, 8].Value = result.LineNumber;

                            rowID++;
                        }
                    }

                    //TODO: 数据透视表更新？？？
                    excel.Workbook.FullCalcOnLoad = true;

                    analysisSheet.PivotTables.ToList().ForEach(table => table?.ToString());
                    analysisSheet.Calculate();
                    excel.Workbook.Calculate();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    excel.Save();
                }
            }
        }

    }
}
