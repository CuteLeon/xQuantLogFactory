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
using xQuantLogFactory.Model.Extensions;
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
        /// 特殊表名列表
        /// </summary>
        private static readonly string[] SpecialSheetNames = new string[]
        {
            FixedDatas.MEMORY_SHEET_NAME,
            FixedDatas.MIDDLEWARE_SHEET_NAME,
            FixedDatas.TRADE_SETTLE_SHEET_NAME,
            FixedDatas.ANALYSIS_SHEET_NAME
        };

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
                    this.ExportCommonSheetEx(excel, argument);

                    this.Tracer?.WriteLine("开始导出保留表数据 ...");
                    this.ExportMemorySheet(excel, argument);
                    this.ExportMiddlewareSheet(excel, argument);
                    this.ExportTradeSettleSheet(excel, argument);

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
        /// 导出通用表数据
        /// </summary>
        /// <param name="excel">excel文档</param>
        /// <param name="argument">任务</param>
        public void ExportCommonSheet(ExcelPackage excel, TaskArgument argument)
        {
            this.Tracer?.WriteLine("开始导出通用表数据 ...");
            foreach (var monitorGroup in argument.MonitorContainerRoot.GetMonitorItems()
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
                    var analysiserResults = new List<GroupAnalysisResult>();
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
                        sourceRange[rowID, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond.ToString() : "匹配失败";
                        sourceRange[rowID, 6].Value = result.StartMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        sourceRange[rowID, 7].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        sourceRange[rowID, 8].Value = result.LogFile.RelativePath;
                        sourceRange[rowID, 9].Value = result.LineNumber;

                        rowID++;
                    }
                }
            }
        }

        /// <summary>
        /// 导出通用表数据
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportCommonSheetEx(ExcelPackage excel, TaskArgument argument)
        {
            /* 按分析结果树分表导出问题：
             * 1. 需要在一个方法内同时维护所有通用数据表对象，若数据总量较大，会产生比原算法更严重的内存压力
             * 2. 分析结果表名若存在跨级，如 分析结果1导出到A表，1的子结果2需要导出到B表，但2的子结果3又需要导出到A表，3也属于1的子结果；
             * 思路：同初始化分析结果树算法，遍历分析结果容器的根节点（仅在容器根节点遍历时更新执行序号），对每个跟节点当做树根遍历，将所有子节点分表导出
             */
            this.Tracer?.WriteLine("开始导出通用表数据 ...");

            this.Tracer?.WriteLine("开始准备通用数据表字典");
            Dictionary<string, ExcelWorksheetPackage> commonWorksheets = new Dictionary<string, ExcelWorksheetPackage>();

            foreach (var sheetName in argument.MonitorContainerRoot.GetMonitorItems()
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
                        argument.MonitorContainerRoot.GetMonitorItems()
                            .Where(monitor => monitor.SheetName == sheetName)
                            .Sum(monitor => monitor.AnalysisResults.Count));

                    commonWorksheets.Add(sheetName, newPackage);
                }
            }

            // 输出监视规则树
            int executeID = 0;
            ExcelWorksheetPackage excelPackage = null;
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.AnalysisResultRoots)
            {
                // 每个分析结果根节点使执行序号自增
                executeID++;

                // 遍历根节点所有子节点输出分析结果数据
                foreach (GroupAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithRoot())
                {
                    // 保留表名的结果不处理
                    if (!SpecialSheetNames.Contains(analysisResult.MonitorItem.SheetName))
                    {
                        if (commonWorksheets.TryGetValue(analysisResult.MonitorItem.SheetName, out excelPackage))
                        {
                            this.WriteCommonAnlysisResult(
                                excelPackage.ExcelRange,
                                excelPackage.RowNumber,
                                analysisResult,
                                executeID);

                            // 行号自增
                            excelPackage.RowNumberIncrease();
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
        /// 写入通用分析结果
        /// </summary>
        /// <param name="range">Excel表区域</param>
        /// <param name="rowNumber">行号</param>
        /// <param name="analysisResult">分析结果</param>
        /// <param name="executeID">执行序号</param>
        private void WriteCommonAnlysisResult(ExcelRange range, int rowNumber, GroupAnalysisResult analysisResult, int executeID)
        {
            range[rowNumber, 1].Value = analysisResult.MonitorItem.PrefixName;
            range[rowNumber, 2].Value = analysisResult.MonitorItem.ParentMonitorItem?.Name;
            range[rowNumber, 3].Value = analysisResult.Version;
            range[rowNumber, 4].Value = executeID;
            range[rowNumber, 5].Value = analysisResult.IsIntactGroup() ? analysisResult.ElapsedMillisecond.ToString() : "匹配失败";
            range[rowNumber, 6].Value = analysisResult.StartMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowNumber, 7].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowNumber, 8].Value = analysisResult.LogFile.RelativePath;
            range[rowNumber, 9].Value = analysisResult.LineNumber;
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
                Rectangle memoryRectangle = new Rectangle(1, 2, 5, argument.MonitorResults.Count);
                using (ExcelRange memoryRange = memoryDataSheet.Cells[memoryRectangle.Top, memoryRectangle.Left, memoryRectangle.Bottom - 1, memoryRectangle.Right - 1])
                {
                    int rowID = memoryRectangle.Top;
                    foreach (var result in argument.AnalysisResults
                        .Where(result => result.MonitorItem.Memory)
                        .OrderBy(result => result.LogTime)
                        .Distinct(new LogResultEqualityComparer<GroupAnalysisResult>()))
                    {
                        memoryRange[rowID, 1].Value = result.MonitorItem?.Name;
                        memoryRange[rowID, 2].Value = result.Version;
                        memoryRange[rowID, 3].Value = result.Client;
                        memoryRange[rowID, 4].Value = result.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        if (result.AnalysisDatas.TryGetValue(FixedDatas.MEMORY_CONSUMED, out object memory))
                        {
                            memoryRange[rowID, 5].Value = memory.ToString();
                        }

                        rowID++;
                    }
                }
            }
        }

        /// <summary>
        /// 导出中间件日志表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportMiddlewareSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet middlewareDataSheet = excel.Workbook.Worksheets[FixedDatas.MIDDLEWARE_SHEET_NAME];
            if (middlewareDataSheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.MIDDLEWARE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.MIDDLEWARE_SHEET_NAME} 表数据 ...");
                Rectangle middlewareRectangle = new Rectangle(1, 2, 9, argument.MiddlewareResults.Count);
                using (ExcelRange middlewareRange = middlewareDataSheet.Cells[middlewareRectangle.Top, middlewareRectangle.Left, middlewareRectangle.Bottom - 1, middlewareRectangle.Right - 1])
                {
                    int rowID = middlewareRectangle.Top;
                    foreach (var result in argument.MiddlewareResults
                        .OrderBy(result => result.LogTime))
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
        }

        /// <summary>
        /// 导出交易清算表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportTradeSettleSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet tradeSettleDataSheet = excel.Workbook.Worksheets[FixedDatas.TRADE_SETTLE_SHEET_NAME];
            if (tradeSettleDataSheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.TRADE_SETTLE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.TRADE_SETTLE_SHEET_NAME} 表数据 ...");
                Rectangle tradeSettleRectangle = new Rectangle(1, 2, 9, argument.MonitorResults.Count);
                using (ExcelRange tradeSettleRange = tradeSettleDataSheet.Cells[tradeSettleRectangle.Top, tradeSettleRectangle.Left, tradeSettleRectangle.Bottom - 1, tradeSettleRectangle.Right - 1])
                {
                    int rowID = tradeSettleRectangle.Top, executeID = 0;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.AnalysisResultRoots)
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (GroupAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithRoot()
                            .Where(result =>
                                result.MonitorItem.Analysiser == AnalysiserTypes.Settle ||
                                result.MonitorItem.Name == FixedDatas.TRADE_SETTLE_SHEET_NAME))
                        {
                            tradeSettleRange[rowID, 1].Value = analysisResult.MonitorItem?.PrefixName;
                            tradeSettleRange[rowID, 2].Value = analysisResult.MonitorItem?.ParentMonitorItem?.Name;
                            tradeSettleRange[rowID, 3].Value = analysisResult.Version;
                            tradeSettleRange[rowID, 4].Value = executeID;
                            tradeSettleRange[rowID, 5].Value = analysisResult.ElapsedMillisecond;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.QSRQ, out object qsrq))
                            {
                                tradeSettleRange[rowID, 6].Value = qsrq;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.DQZH, out object dqzh))
                            {
                                tradeSettleRange[rowID, 7].Value = dqzh;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.WBZH, out object wbzh))
                            {
                                tradeSettleRange[rowID, 8].Value = wbzh;
                            }

                            tradeSettleRange[rowID, 9].Value = analysisResult.StartMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            tradeSettleRange[rowID, 10].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            tradeSettleRange[rowID, 11].Value = analysisResult.LogFile?.RelativePath;
                            tradeSettleRange[rowID, 12].Value = analysisResult.LineNumber;

                            rowID++;
                        }
                    }
                }
            }
        }
    }
}
