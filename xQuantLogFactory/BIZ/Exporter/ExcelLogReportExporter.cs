using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using OfficeOpenXml;

using xQuantLogFactory.BIZ.Processer;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.EqualityComparer;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Report;
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
        /// 保留表名列表
        /// </summary>
        private static readonly string[] SpecialSheetNames = new string[]
        {
            FixedDatas.MEMORY_SHEET_NAME,
            FixedDatas.PERFORMANCE_MONITOR_SHEET_NAME,
            FixedDatas.PERFORMANCE_PARSE_SHEET_NAME,
            FixedDatas.TRADE_SETTLE_SHEET_NAME,
            FixedDatas.ANALYSIS_SHEET_NAME,
            FixedDatas.CORE_SERVICE_SHEET_NAME,
            FixedDatas.FORM_SHEET_NAME,
            FixedDatas.REPORT_SHEET_NAME,
            FixedDatas.CACHE_SHEET_NAME,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelLogReportExporter"/> class.
        /// </summary>
        /// <param name="tracer"></param>
        public ExcelLogReportExporter(ITracer tracer)
            : base(tracer)
        {
        }

        /// <summary>
        /// 导出Excel报告
        /// </summary>
        /// <param name="reportPath">报告路径</param>
        /// <param name="argument">任务参数</param>
        public void ExportReport(string reportPath, TaskArgument argument)
        {
            // 导出模板
            try
            {
                this.Tracer?.WriteLine("正在拷贝 Excel 报告模板...");
                File.Copy(ConfigHelper.ExcelReportTempletPath, reportPath, true);
            }
            catch
            {
                throw;
            }

            this.Tracer?.WriteLine("正在连接 Excel 报告文件...");

            // 连接Excel文件
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(reportPath)))
            {
                try
                {
                    this.Tracer?.WriteLine("正在设置 Excel 报告属性信息 ...");
                    OfficeProperties properties = excel.Workbook.Properties;
                    properties.Author = $"xQuant日志分析工具 - {Application.ProductVersion}";
                    properties.Category = "xQuant日志分析报告";
                    properties.Title = $"xQuant日志分析报告-{argument.TaskID}";
                    properties.Comments = properties.Title;
                    properties.Company = "xQuant";
                    properties.Created = DateTime.Now;
                    properties.Manager = properties.Author;
                    properties.Subject = properties.Title;

                    // 导出通用表数据
                    this.ExportTerminalCommonSheetEx(excel, argument);

                    this.Tracer?.WriteLine("开始导出保留表数据 ...");
                    this.ExportMemorySheet(excel, argument);
                    this.ExportPerformanceMonitorSheet(excel, argument);
                    this.ExportPerformanceParseSheet(excel, argument);
                    this.ExportTradeClearingSheet(excel, argument);
                    this.ExportCoreServiceSheet(excel, argument);
                    this.ExportFormSheet(excel, argument);
                    this.ExportReportSheet(excel, argument);
                    this.ExportCacheSheet(excel, argument);

                    /* 更新数据透视表
                    ExcelWorksheet analysisSheet = excel.Workbook.Worksheets["分析"];
                    if (analysisSheet != null)
                    {
                        excel.Workbook.FullCalcOnLoad = true;
                        analysisSheet.PivotTables.ToList().ForEach(table => table?.ToString());
                        analysisSheet.Calculate();
                        excel.Workbook.Calculate();
                    }
                     */
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

        /// <summary>
        /// 导出缓存统计数据
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportCacheSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.CACHE_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.CACHE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.CACHE_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 12, argument.TerminalMonitorResults.Count);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf()
                            .Where(result => result.MonitorItem.SheetName == FixedDatas.CACHE_SHEET_NAME))
                        {
                            range[rowID, 1].Value = analysisResult.MonitorItem.PrefixName;
                            range[rowID, 2].Value = analysisResult.MonitorItem.ParentMonitorItem?.Name;
                            range[rowID, 3].Value = analysisResult.Version;
                            range[rowID, 4].Value = executeID;
                            range[rowID, 5].Value = analysisResult.IsIntactGroup() ? analysisResult.ElapsedMillisecond : 0.0;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.RESOURCE_NAME, out object name))
                            {
                                range[rowID, 6].Value = name;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.COUNT, out object count))
                            {
                                range[rowID, 7].Value = count;
                            }

                            range[rowID, 8].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 9].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 10].Value = analysisResult.LogFile.RelativePath;
                            range[rowID, 11].Value = analysisResult.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出报表日志
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportReportSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.REPORT_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.REPORT_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.REPORT_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 11, argument.TerminalMonitorResults.Count);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf()
                            .Where(result => result.MonitorItem.SheetName == FixedDatas.REPORT_SHEET_NAME))
                        {
                            range[rowID, 1].Value = analysisResult.MonitorItem.PrefixName;
                            range[rowID, 2].Value = analysisResult.MonitorItem.ParentMonitorItem?.Name;
                            range[rowID, 3].Value = analysisResult.Version;
                            range[rowID, 4].Value = executeID;
                            range[rowID, 5].Value = analysisResult.IsIntactGroup() ? analysisResult.ElapsedMillisecond : 0.0;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.REPORT_CODE, out object code))
                            {
                                range[rowID, 6].Value = code;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.REPORT_NAME, out object name))
                            {
                                range[rowID, 7].Value = name;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.QUERY_PARAM, out object param))
                            {
                                range[rowID, 8].Value = param;
                            }

                            range[rowID, 9].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 10].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 11].Value = analysisResult.LogFile.RelativePath;
                            range[rowID, 12].Value = analysisResult.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出窗体日志
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportFormSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.FORM_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.FORM_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.FORM_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 11, argument.TerminalMonitorResults.Count);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf()
                            .Where(result => result.MonitorItem.SheetName == FixedDatas.FORM_SHEET_NAME))
                        {
                            range[rowID, 1].Value = analysisResult.MonitorItem.PrefixName;
                            range[rowID, 2].Value = analysisResult.MonitorItem.ParentMonitorItem?.Name;
                            range[rowID, 3].Value = analysisResult.Version;
                            range[rowID, 4].Value = executeID;
                            range[rowID, 5].Value = analysisResult.IsIntactGroup() ? analysisResult.ElapsedMillisecond : 0.0;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.MODULE_CODE, out object code))
                            {
                                range[rowID, 6].Value = code;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.FORM_NAME, out object name))
                            {
                                range[rowID, 7].Value = name;
                            }

                            range[rowID, 8].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 9].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 10].Value = analysisResult.LogFile.RelativePath;
                            range[rowID, 11].Value = analysisResult.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出Core服务日志
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportCoreServiceSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.CORE_SERVICE_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.CORE_SERVICE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.CORE_SERVICE_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 11, argument.TerminalMonitorResults.Count);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots
                        .Where(root => root.IsIntactGroup()))
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf()
                            .Where(result => result.MonitorItem.SheetName == FixedDatas.CORE_SERVICE_SHEET_NAME))
                        {
                            range[rowID, 1].Value = analysisResult.Version;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.CORE_SERVICE_NAME, out object service))
                            {
                                range[rowID, 2].Value = service;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.EXECUTE_INDEX, out object index))
                            {
                                range[rowID, 3].Value = index;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.TRIGGER, out object trigger))
                            {
                                range[rowID, 4].Value = trigger;
                            }

                            range[rowID, 5].Value = analysisResult.ElapsedMillisecond;
                            range[rowID, 6].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 7].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 8].Value = analysisResult.LogFile?.RelativePath;
                            range[rowID, 9].Value = analysisResult.StartMonitorResult?.LineNumber;
                            range[rowID, 10].Value = analysisResult.FinishMonitorResult?.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出客户端和服务端通用表数据
        /// </summary>
        /// <param name="excel">excel文档</param>
        /// <param name="argument">任务</param>
        [Obsolete]
        public void ExportTerminalCommonSheet(ExcelPackage excel, TaskArgument argument)
        {
            this.Tracer?.WriteLine("开始导出通用表数据 ...");
            foreach (var monitorGroup in argument.MonitorContainerRoot.GetTerminalMonitorItems()
                .GroupBy(monitor => monitor.SheetName))
            {
                // 特殊表名单独处理
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

                // 通用表列头格式需保持与原始数据表一致
                Rectangle sheetRectangle = new Rectangle(1, 2, 9, monitorGroup.Sum(monitor => monitor.AnalysisResults.Count));
                using (ExcelRange sourceRange = worksheet.Cells[sheetRectangle.Top, sheetRectangle.Left, sheetRectangle.Bottom - 1, sheetRectangle.Right - 1])
                {
                    int rowID = sheetRectangle.Top, executeID = 0;

                    // 合并所有分析结果数据
                    var analysiserResults = new List<TerminalAnalysisResult>();
                    monitorGroup.Select(monitor => monitor.AnalysisResults).ToList()
                        .ForEach(resultList => analysiserResults.AddRange(resultList));

                    foreach (var result in analysiserResults
                        .OrderBy(result => (result.LogFile?.RelativePath, result.LineNumber)))
                    {
                        // 遇到分析结果根节点，执行序号自增
                        if (result.ParentAnalysisResult == null)
                        {
                            executeID++;
                        }

                        sourceRange[rowID, 1].Value = result.MonitorItem.PrefixName;
                        sourceRange[rowID, 2].Value = result.MonitorItem.ParentMonitorItem?.Name;
                        sourceRange[rowID, 3].Value = result.Version;
                        sourceRange[rowID, 4].Value = executeID;
                        sourceRange[rowID, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond : 0.0;
                        sourceRange[rowID, 6].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                        sourceRange[rowID, 7].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        sourceRange[rowID, 8].Value = result.LogFile.RelativePath;
                        sourceRange[rowID, 9].Value = result.LineNumber;

                        rowID++;
                    }
                }
            }
        }

        /// <summary>
        /// 导出客户端和服务端通用表格式数据
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportTerminalCommonSheetEx(ExcelPackage excel, TaskArgument argument)
        {
            /* 按分析结果树分表导出问题：
             * 1. 需要在一个方法内同时维护所有通用数据表对象，若数据总量较大，会产生比原算法更严重的内存压力
             * 2. 分析结果表名若存在跨级，如 分析结果1导出到A表，1的子结果2需要导出到B表，但2的子结果3又需要导出到A表，3也属于1的子结果；
             * 思路：同初始化分析结果树算法，遍历分析结果容器的根节点（仅在容器根节点遍历时更新执行序号），对每个跟节点当做树根遍历，将所有子节点分表导出
             */
            this.Tracer?.WriteLine("开始导出通用表数据 ...");

            this.Tracer?.WriteLine("开始准备通用数据表字典");
            Dictionary<string, ExcelWorksheetPackage> commonWorksheets = new Dictionary<string, ExcelWorksheetPackage>();

            foreach (var sheetName in argument.MonitorContainerRoot.GetTerminalMonitorItems()
                .Where(monitor => monitor.AnalysisResults.Count > 0)
                .Select(monitor => monitor.SheetName).Distinct())
            {
                if (SpecialSheetNames.Contains(sheetName))
                {
                    // 保留表名不在此处处理
                    continue;
                }

                ExcelWorksheet worksheet = excel.Workbook.Worksheets[sheetName];
                if (worksheet == null)
                {
                    this.Tracer?.WriteLine($"未发现名称为 {sheetName} 的数据表，将跳过导出此表数据");
                    continue;
                }
                else
                {
                    ExcelWorksheetPackage newPackage = this.CreateCommonWorksheetPackage(
                        worksheet,
                        argument.MonitorContainerRoot.GetTerminalMonitorItems()
                            .Where(monitor => monitor.SheetName == sheetName)
                            .Sum(monitor => monitor.AnalysisResults.Count));

                    commonWorksheets.Add(sheetName, newPackage);
                }
            }

            // 输出监视规则树
            int executeID = 0;
            ExcelWorksheetPackage excelPackage = null;
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                // 每个分析结果根节点使执行序号自增
                executeID++;

                // 遍历根节点所有子节点输出分析结果数据
                foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf())
                {
                    // 保留表名的结果不处理
                    if (!SpecialSheetNames.Contains(analysisResult.MonitorItem.SheetName))
                    {
                        if (commonWorksheets.TryGetValue(analysisResult.MonitorItem.SheetName, out excelPackage))
                        {
                            this.WriteCommonAnlysisResult(
                                excelPackage.ExcelRange,
                                excelPackage.RowID,
                                analysisResult,
                                executeID);

                            // 行号自增
                            excelPackage.RowIDIncrease();
                        }
                        else
                        {
                            // 未找到此表
                        }
                    }
                }
            }

            // 清理垃圾
            commonWorksheets.Values.ToList().ForEach(package => package.Dispose());
            commonWorksheets.Clear();
        }

        /// <summary>
        /// 导出内存数据表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportMemorySheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet memoryDataSheet = excel.Workbook.Worksheets[FixedDatas.MEMORY_SHEET_NAME];
            if (memoryDataSheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.MEMORY_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.MEMORY_SHEET_NAME} 表数据 ...");
                Rectangle memoryRectangle = new Rectangle(1, 2, 5, argument.TerminalMonitorResults.Count);
                using (ExcelRange memoryRange = memoryDataSheet.Cells[memoryRectangle.Top, memoryRectangle.Left, memoryRectangle.Bottom - 1, memoryRectangle.Right - 1])
                {
                    int rowID = memoryRectangle.Top;
                    foreach (var result in argument.TerminalAnalysisResults
                        .Where(result => result.MonitorItem.Memory)
                        .Distinct(new TerminalAnalysisResultEqualityComparer<TerminalAnalysisResult>()))
                    {
                        memoryRange[rowID, 1].Value = result.MonitorItem?.Name;
                        memoryRange[rowID, 2].Value = result.Version;
                        memoryRange[rowID, 3].Value = result.Client;
                        memoryRange[rowID, 4].Value = result.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                        if (result.AnalysisDatas.TryGetValue(FixedDatas.MEMORY_CONSUMED, out object memory))
                        {
                            memoryRange[rowID, 5].Value = memory;
                        }

                        rowID++;
                    }
                }
            }
        }

        /// <summary>
        /// 导出Performance监视结果表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportPerformanceMonitorSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.PERFORMANCE_MONITOR_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.PERFORMANCE_MONITOR_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.PERFORMANCE_MONITOR_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 10, argument.PerformanceAnalysisResults.Count);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.PerformanceAnalysisResultRoots)
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (PerformanceAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf())
                        {
                            range[rowID, 1].Value = analysisResult.IPAddress;
                            range[rowID, 2].Value = analysisResult.UserCode;
                            range[rowID, 3].Value = analysisResult.MonitorItem.PrefixName;
                            range[rowID, 4].Value = analysisResult.MonitorItem.ParentMonitorItem?.Name;
                            range[rowID, 5].Value = executeID;
                            range[rowID, 6].Value = analysisResult.IsIntactGroup() ? analysisResult.ElapsedMillisecond : 0.0;
                            range[rowID, 7].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 8].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 9].Value = analysisResult.LogFile.RelativePath;
                            range[rowID, 10].Value = analysisResult.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 导出Performance解析结果表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportPerformanceParseSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet dataSheet = excel.Workbook.Worksheets[FixedDatas.PERFORMANCE_PARSE_SHEET_NAME];
            if (dataSheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.PERFORMANCE_PARSE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                /* TODO: [依赖限制] Excel 2010限制最大导出记录数量 = ExcelPackage.MaxRows = 1048576
                 * https://github.com/JanKallman/EPPlus/blob/master/EPPlus/ExcelPackage.cs
                 */
                int rowCount = argument.PerformanceParseResults.Count,
                     maxRow = ExcelPackage.MaxRows - 1;

                /* 懒加载类型
                 * Where(PerformanceTypes.Start)：适用于统计方法调用次数
                 * Where(PerformanceTypes.Finish)：适用于统计方法平均耗时
                 */
                IEnumerable<PerformanceMonitorResult> performanceParseResults = argument.PerformanceParseResults.Where(result => result.PerformanceType == PerformanceTypes.Start);

                if (rowCount > maxRow)
                {
                    this.Tracer.WriteLine(
                        "\tPerformance解析结果数量 {0} 大于支持的最大导出数量 {1} ，将对多余数据截断 ...",
                        argument.PerformanceParseResults.Count,
                        maxRow);

                    rowCount = maxRow;
                    performanceParseResults = performanceParseResults.Take(rowCount);
                }

                this.Tracer?.WriteLine($"正在写入 {FixedDatas.PERFORMANCE_PARSE_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 11, rowCount);
                using (ExcelRange range = dataSheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top;
                    foreach (var result in performanceParseResults
                        .OrderBy(result => (result.LogTime, result.LineNumber)))
                    {
                        range[rowID, 1].Value = result.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        range[rowID, 2].Value = result.IPAddress;
                        range[rowID, 3].Value = result.UserCode;
                        range[rowID, 4].Value = result.StartTime;
                        range[rowID, 5].Value = result.Elapsed;
                        range[rowID, 6].Value = result.RequestURI;
                        range[rowID, 7].Value = result.MethodName;
                        range[rowID, 8].Value = result.StreamLength;
                        range[rowID, 9].Value = result.Message;
                        range[rowID, 10].Value = result.LogFile.RelativePath;
                        range[rowID, 11].Value = result.LineNumber;

                        rowID++;
                    }
                }
            }
        }

        /// <summary>
        /// 导出交易清算表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportTradeClearingSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.TRADE_SETTLE_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.TRADE_SETTLE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.TRADE_SETTLE_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 13, argument.TerminalMonitorResults.Count);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots
                        .Where(root => root.IsIntactGroup()))
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf()
                            .Where(result => result.MonitorItem.SheetName == FixedDatas.TRADE_SETTLE_SHEET_NAME))
                        {
                            range[rowID, 1].Value = analysisResult.MonitorItem?.PrefixName;
                            range[rowID, 2].Value = analysisResult.MonitorItem?.ParentMonitorItem?.Name;
                            range[rowID, 3].Value = analysisResult.Version;
                            range[rowID, 4].Value = executeID;
                            range[rowID, 5].Value = analysisResult.ElapsedMillisecond;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.QSRQ, out object qsrq))
                            {
                                range[rowID, 6].Value = qsrq;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.DQZH, out object dqzh))
                            {
                                range[rowID, 7].Value = dqzh;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.WBZH, out object wbzh))
                            {
                                range[rowID, 8].Value = wbzh;
                            }

                            range[rowID, 9].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 10].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 11].Value = analysisResult.LogFile?.RelativePath;

                            // tradeSettleRange[rowID, 12].Value = analysisResult.LineNumber;
                            range[rowID, 12].Value = analysisResult.StartMonitorResult?.LineNumber;
                            range[rowID, 13].Value = analysisResult.FinishMonitorResult?.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 写入通用分析结果
        /// </summary>
        /// <param name="range">Excel表区域</param>
        /// <param name="rowID">行号</param>
        /// <param name="analysisResult">分析结果</param>
        /// <param name="executeID">执行序号</param>
        private void WriteCommonAnlysisResult(ExcelRange range, int rowID, TerminalAnalysisResult analysisResult, int executeID)
        {
            range[rowID, 1].Value = analysisResult.MonitorItem.PrefixName;
            range[rowID, 2].Value = analysisResult.MonitorItem.ParentMonitorItem?.Name;
            range[rowID, 3].Value = analysisResult.Version;
            range[rowID, 4].Value = executeID;
            range[rowID, 5].Value = analysisResult.IsIntactGroup() ? analysisResult.ElapsedMillisecond : 0.0;
            range[rowID, 6].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowID, 7].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowID, 8].Value = analysisResult.LogFile.RelativePath;
            range[rowID, 9].Value = analysisResult.LineNumber;
        }

        /// <summary>
        /// 创建通用表数据包
        /// </summary>
        /// <param name="worksheet">数据表</param>
        /// <param name="maxRow">最大行数</param>
        /// <returns></returns>
        private ExcelWorksheetPackage CreateCommonWorksheetPackage(ExcelWorksheet worksheet, int maxRow)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            Rectangle sheetRectangle = new Rectangle(1, 2, 9, maxRow);
            ExcelRange sheetRange = worksheet.Cells[sheetRectangle.Top, sheetRectangle.Left, sheetRectangle.Bottom - 1, sheetRectangle.Right - 1];
            ExcelWorksheetPackage package = new ExcelWorksheetPackage(worksheet, sheetRectangle.Top, sheetRange);

            return package;
        }
    }
}
