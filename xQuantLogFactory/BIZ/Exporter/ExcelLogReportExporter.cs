using System;
using System.Drawing;
using System.IO;
using System.Linq;

using OfficeOpenXml;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.EqualityComparer;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Result;
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
                    ExcelWorksheet memoryDataSheet = excel.Workbook.Worksheets["内存"];
                    ExcelWorksheet analysisSheet = excel.Workbook.Worksheets["分析"];

                    Rectangle sourceRectangle = new Rectangle(1, 2, 5, argument.AnalysisResults.Count);
                    using (ExcelRange sourceRange = sourceDataSheet.Cells[sourceRectangle.Top, sourceRectangle.Left, sourceRectangle.Bottom - 1, sourceRectangle.Right - 1])
                    {
                        int rowID = sourceRectangle.Top, executeID = 0;
                        foreach (var result in argument.AnalysisResults
                            .Where(result => !result.MonitorItem?.NotSource ?? false)
                            .OrderBy(result => (result.LogFile?.FileID, result.LineNumber))
                            )
                        {
                            if (result.MonitorItem?.ParentMonitorItem == null) executeID++;

                            sourceRange[rowID, 1].Value = result.MonitorItem?.Name;
                            sourceRange[rowID, 2].Value = result.MonitorItem?.ParentMonitorItem?.Name;
                            sourceRange[rowID, 3].Value = result.Version;
                            sourceRange[rowID, 4].Value = executeID;
                            sourceRange[rowID, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond.ToString() : "匹配失败";

                            rowID++;
                        }
                    }

                    Rectangle memoryRectangle = new Rectangle(1, 2, 6, argument.MonitorResults.Count);
                    using (ExcelRange memoryRange = memoryDataSheet.Cells[memoryRectangle.Top, memoryRectangle.Left, memoryRectangle.Bottom - 1, memoryRectangle.Right - 1])
                    {
                        int rowID = memoryRectangle.Top;
                        foreach (var result in argument.MonitorResults
                            .Where(result => result.MemoryConsumed != null)
                            //(result => (result.MonitorItem?.ItemID, result.Version, result.Client)
                            .OrderBy(result => result.LogTime)
                            //存在：同一行日志被多个监视内存的监视规则匹配到而造成同一条日志产生多个内存数据，去重
                            .Distinct(new LogResultEqualityComparer<MonitorResult>())
                            )
                        {
                            memoryRange[rowID, 1].Value = result.MonitorItem?.Name;
                            memoryRange[rowID, 2].Value = result.Version;
                            memoryRange[rowID, 3].Value = result.Client;
                            memoryRange[rowID, 4].Value = result.MemoryConsumed;
                            memoryRange[rowID, 5].Value = result.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            memoryRange[rowID, 6].Value = result.GroupType.ToString();

                            rowID++;
                        }
                    }

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
