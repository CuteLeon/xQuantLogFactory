using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using OfficeOpenXml;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.EqualityComparer;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Exporter
{
    /// <summary>
    /// Excel 表格导出器
    /// </summary>
    public class ExcelLogReportExporter : LogProcesserBase, ILogReportExporter
    {

        /// <summary>
        /// 特殊表名列表
        /// </summary>
        private readonly static string[] SpecialSheetNames = new string[] { "内存", "中间件日志", "分析" };

        public ExcelLogReportExporter(ITracer tracer) : base(tracer) { }

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
                this.Tracer?.WriteLine("正在拷贝 Excel 报告模板...");
                File.Copy(ConfigHelper.ExcelReportTempletPath, reportPath, true);
            }
            catch { throw; }

            this.Tracer?.WriteLine("正在连接 Excel 报告文件...");
            //连接Excel文件
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(reportPath)))
            {
                try
                {
                    this.Tracer?.WriteLine("正在设置 Excel 报告属性信息 ...");
                    OfficeProperties properties = excel.Workbook.Properties;
                    properties.Author = "xQuant日志分析工具";
                    properties.Category = "xQuant日志分析报告";
                    properties.Comments = $"xQuant日志分析报告-{argument.TaskID}";
                    properties.Company = "xQuant";
                    properties.Created = DateTime.Now;
                    properties.Manager = "xQuant日志分析工具";
                    properties.Subject = $"xQuant日志分析报告-{argument.TaskID}";
                    properties.Title = $"xQuant日志分析报告-{argument.TaskID}";

                    //按表名分组导出
                    this.Tracer?.WriteLine("开始导出通用表数据 ...");
                    foreach (var monitorGroup in argument.MonitorRoot.GetMonitorItems().GroupBy(monitor => monitor.SheetName))
                    {
                        //特殊表名单独处理
                        if (SpecialSheetNames.Contains(monitorGroup.Key))
                        {
                            this.Tracer?.WriteLine($"表名 [{monitorGroup.Key}] 为保留表名，延迟导出 ...");
                            continue;
                        }

                        ExcelWorksheet worksheet = excel.Workbook.Worksheets[monitorGroup.Key];
                        if (worksheet == null)
                        {
                            this.Tracer?.WriteLine($"未发现名称为 {monitorGroup.Key} 的数据表，跳过导出 ...");
                            continue;
                        }

                        this.Tracer?.WriteLine($"正在写入 {monitorGroup.Key} 表数据 ...");
                        //通用表列头格式需保持与原始数据表一致
                        Rectangle sheetRectangle = new Rectangle(1, 2, 9, monitorGroup.Sum(monitor => monitor.AnalysisResults.Count));
                        using (ExcelRange sourceRange = worksheet.Cells[sheetRectangle.Top, sheetRectangle.Left, sheetRectangle.Bottom - 1, sheetRectangle.Right - 1])
                        {
                            //TODO: 分表导出如何分析执行序号：为监视规则设置属性，遇到即更新序号？
                            int rowID = sheetRectangle.Top, executeID = 0;

                            //合并所有分析结果数据
                            var analysiserResults = new List<GroupAnalysisResult>();
                            monitorGroup.Select(monitor => monitor.AnalysisResults).ToList()
                                .ForEach(resultList => analysiserResults.AddRange(resultList));

                            foreach (var result in analysiserResults
                                .OrderBy(result => (result.LogFile?.FileID, result.LineNumber))
                                )
                            {
                                if (result.MonitorItem != null)
                                {
                                    sourceRange[rowID, 1].Value = result.MonitorItem.Name.PadLeft(result.MonitorItem.GetLayerDepth() + result.MonitorItem.Name.Length, '-');
                                    sourceRange[rowID, 2].Value = result.MonitorItem.ParentMonitorItem?.Name;
                                }
                                sourceRange[rowID, 3].Value = result.Version;
                                sourceRange[rowID, 4].Value = executeID;
                                sourceRange[rowID, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond.ToString() : "匹配失败";
                                sourceRange[rowID, 6].Value = result.StartMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                sourceRange[rowID, 7].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                sourceRange[rowID, 8].Value = result.LogFile.RelativePath;
                                sourceRange[rowID, 9].Value = result.FirstResultOrDefault()?.LineNumber;

                                rowID++;
                            }
                        }
                    }

                    this.Tracer?.WriteLine("开始导出保留表数据 ...");
                    ExcelWorksheet memoryDataSheet = excel.Workbook.Worksheets["内存"];
                    if (memoryDataSheet == null)
                    {
                        this.Tracer?.WriteLine("未发现内存数据表，写入失败！");
                    }
                    else
                    {
                        this.Tracer?.WriteLine("正在写入 内存 表数据 ...");
                        Rectangle memoryRectangle = new Rectangle(1, 2, 6, argument.MonitorResults.Count);
                        using (ExcelRange memoryRange = memoryDataSheet.Cells[memoryRectangle.Top, memoryRectangle.Left, memoryRectangle.Bottom - 1, memoryRectangle.Right - 1])
                        {
                            int rowID = memoryRectangle.Top;
                            foreach (var result in argument.MonitorResults
                                .Where(result => result.MemoryConsumed != null)
                                .OrderBy(result => result.LogTime)
                                .Distinct(new LogResultEqualityComparer<MonitorResult>())
                                )
                            {
                                memoryRange[rowID, 1].Value = result.MonitorItem?.Name;
                                memoryRange[rowID, 2].Value = result.Version;
                                memoryRange[rowID, 3].Value = result.Client;
                                memoryRange[rowID, 4].Value = result.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                memoryRange[rowID, 5].Value = result.MemoryConsumed;
                                memoryRange[rowID, 6].Value = result.GroupType.ToString();

                                rowID++;
                            }
                        }
                    }

                    ExcelWorksheet middlewareDataSheet = excel.Workbook.Worksheets["中间件日志"];
                    if (middlewareDataSheet == null)
                    {
                        this.Tracer?.WriteLine("未发现中间件数据表，写入失败！");
                    }
                    else
                    {
                        this.Tracer?.WriteLine("正在写入 中间件日志 表数据 ...");
                        Rectangle middlewareRectangle = new Rectangle(1, 2, 9, argument.MiddlewareResults.Count);
                        using (ExcelRange middlewareRange = middlewareDataSheet.Cells[middlewareRectangle.Top, middlewareRectangle.Left, middlewareRectangle.Bottom - 1, middlewareRectangle.Right - 1])
                        {
                            int rowID = middlewareRectangle.Top;
                            foreach (var result in argument.MiddlewareResults
                                .OrderBy(result => result.LogTime)
                                )
                            {
                                middlewareRange[rowID, 1].Value = result.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                middlewareRange[rowID, 2].Value = result.Client;
                                middlewareRange[rowID, 3].Value = result.UserCode;
                                middlewareRange[rowID, 4].Value = result.StartTime;
                                middlewareRange[rowID, 5].Value = result.Elapsed;
                                middlewareRange[rowID, 6].Value = result.RequestURI;
                                middlewareRange[rowID, 7].Value = result.MethodName;
                                middlewareRange[rowID, 8].Value = result.StreamLength;
                                middlewareRange[rowID, 9].Value = result.Message;

                                rowID++;
                            }
                        }
                    }

                    ExcelWorksheet analysisSheet = excel.Workbook.Worksheets["分析"];
                    if (analysisSheet != null)
                    {
                        excel.Workbook.FullCalcOnLoad = true;
                        analysisSheet.PivotTables.ToList().ForEach(table => table?.ToString());
                        analysisSheet.Calculate();
                        excel.Workbook.Calculate();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    excel.Save();
                    this.Tracer?.WriteLine("Excel 报告文档关闭");
                }
            }
        }

    }
}
