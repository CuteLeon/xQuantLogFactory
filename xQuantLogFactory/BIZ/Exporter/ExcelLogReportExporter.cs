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
        /// 获取分析结果委托
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        private delegate List<(int ExecuteID, TResult Result)> GetAnalysisResultsDelegate<TResult>(TaskArgument argument)
            where TResult : IAnalysisResult;

        /// <summary>
        /// 获取解析结果委托
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        private delegate List<TResult> GetMonitorResultsDelegate<TResult>(TaskArgument argument)
            where TResult : IMonitorResult;

        /// <summary>
        /// 导出分析结果委托
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private delegate void ExportAnalysisResultDelegate<TResult>(ExcelRange range, int rowId, int executeId, TResult result)
            where TResult : IAnalysisResult;

        /// <summary>
        /// 导出解析结果委托
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private delegate void ExportMonitorResultDelegate<TResult>(ExcelRange range, int rowId, TResult result)
            where TResult : IMonitorResult;

        /// <summary>
        /// 保留表名列表
        /// </summary>
        private static readonly string[] SpecialSheetNames = new string[]
        {
            FixedDatas.MEMORY_SHEET_NAME,
            FixedDatas.PERFORMANCE_Analysiser_SHEET_NAME,
            FixedDatas.PERFORMANCE_PARSE_SHEET_NAME,
            FixedDatas.TRADE_SETTLE_SHEET_NAME,
            FixedDatas.ANALYSIS_SHEET_NAME,
            FixedDatas.CORE_SERVICE_SHEET_NAME,
            FixedDatas.FORM_SHEET_NAME,
            FixedDatas.REPORT_SHEET_NAME,
            FixedDatas.CACHE_SHEET_NAME,
            FixedDatas.LIMIT_CHECK_SHEET_NAME,
            FixedDatas.CLIENT_MESSAGE_SHEET_NAME,
            FixedDatas.SQL_SHEET_NAME,
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
                    properties.Comments = $"日志目录：{argument.LogDirectory}\n监视规则：{argument.MonitorFileName}";
                    properties.Company = "xQuant";
                    properties.Created = DateTime.Now;
                    properties.Manager = properties.Author;
                    properties.Subject = properties.Title;

                    // 导出通用表数据
                    this.ExportTerminalCommonSheetEx(excel, argument);

                    this.Tracer?.WriteLine("开始导出保留表数据 ...");
                    this.ExportSheet(excel, FixedDatas.PERFORMANCE_PARSE_SHEET_NAME, argument, this.GetPerformanceMonitorResults, this.ExportPerformanceMonitorResult);
                    this.ExportSheet(excel, FixedDatas.SQL_SHEET_NAME, argument, this.GetSQLAnalysisResults, this.ExportSQLAnalysiserResult);

                    this.ExportMemorySheet(excel, argument);
                    this.ExportPerformanceAnalysiserSheet(excel, argument);
                    this.ExportTradeClearingSheet(excel, argument);
                    this.ExportCoreServiceSheet(excel, argument);
                    this.ExportFormSheet(excel, argument);
                    this.ExportReportSheet(excel, argument);
                    this.ExportCacheSheet(excel, argument);
                    this.ExportLimitCheck(excel, argument);
                    this.ExportClientMessageSheet(excel, argument);

                    this.Tracer?.WriteLine("正在保存数据到 Excel 文件，请稍等...");
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

        #region SheetRender

        /// <summary>
        /// 导出解析结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="excel"></param>
        /// <param name="sheetName"></param>
        /// <param name="argument"></param>
        /// <param name="getMonitorResultsAction"></param>
        /// <param name="exportMonitorResultsAction"></param>
        private void ExportSheet<TResult>(
            ExcelPackage excel,
            string sheetName,
            TaskArgument argument,
            GetMonitorResultsDelegate<TResult> getMonitorResultsAction,
            ExportMonitorResultDelegate<TResult> exportMonitorResultsAction)
            where TResult : IMonitorResult
        {
            var results = getMonitorResultsAction?.Invoke(argument);
            if (results.Count == 0)
            {
                return;
            }

            int resultIndex = 0;
            int sheetIndex = 0;

            while (resultIndex < results.Count)
            {
                // 获取并克隆新表，防止写入数据到Sheet之后再克隆会因为写入的数据而耗时
                ExcelWorksheet sheet = this.GetSheet(excel, sheetName, sheetIndex);
                if (sheet == null)
                {
                    this.Tracer?.WriteLine($"未发现 {sheet.Name} 数据表，写入失败！");
                    return;
                }
                else
                {
                    this.Tracer?.WriteLine($"正在写入 {sheet.Name} 数据表 ...");
                }

                this.CloneSheet(excel, sheet, sheetName, ++sheetIndex);

                Rectangle rectangle = new Rectangle(1, 2, 32, FixedDatas.ExcelMaxRowCount);
                int rowId = rectangle.Top;
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    foreach (var result in results.Skip(resultIndex).Take(FixedDatas.ExcelMaxRowCount))
                    {
                        exportMonitorResultsAction(range, rowId, result);
                        rowId++;
                        resultIndex++;
                    }
                }
            }

            if (sheetIndex > 0)
            {
                // 移除最后创建的纯净表
                this.RmoveSheet(excel, sheetName, sheetIndex);
            }
        }

        /// <summary>
        /// 导出分析结果表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="excel"></param>
        /// <param name="sheetName"></param>
        /// <param name="argument"></param>
        /// <param name="getAnalysisResultsAction"></param>
        /// <param name="exportAnalysisResultsAction"></param>
        private void ExportSheet<TResult>(
            ExcelPackage excel,
            string sheetName,
            TaskArgument argument,
            GetAnalysisResultsDelegate<TResult> getAnalysisResultsAction,
            ExportAnalysisResultDelegate<TResult> exportAnalysisResultsAction)
            where TResult : IAnalysisResult
        {
            var results = getAnalysisResultsAction?.Invoke(argument);
            if (results.Count == 0)
            {
                return;
            }

            int resultIndex = 0;
            int sheetIndex = 0;

            while (resultIndex < results.Count)
            {
                // 获取并克隆新表，防止写入数据到Sheet之后再克隆会因为写入的数据而耗时
                ExcelWorksheet sheet = this.GetSheet(excel, sheetName, sheetIndex);
                if (sheet == null)
                {
                    this.Tracer?.WriteLine($"未发现 {sheet.Name} 数据表，写入失败！");
                    return;
                }
                else
                {
                    this.Tracer?.WriteLine($"正在写入 {sheet.Name} 数据表 ...");
                }

                this.CloneSheet(excel, sheet, sheetName, ++sheetIndex);

                Rectangle rectangle = new Rectangle(1, 2, 32, FixedDatas.ExcelMaxRowCount);
                int rowId = rectangle.Top;
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    foreach (var (executeID, result) in results.Skip(resultIndex).Take(FixedDatas.ExcelMaxRowCount))
                    {
                        exportAnalysisResultsAction(range, rowId, executeID, result);
                        rowId++;
                        resultIndex++;
                    }
                }
            }

            if (sheetIndex > 0)
            {
                // 移除最后创建的纯净表
                this.RmoveSheet(excel, sheetName, sheetIndex);
            }
        }

        /// <summary>
        /// 克隆新表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="originalExcel"></param>
        /// <param name="sheetName"></param>
        /// <param name="newIndex"></param>
        private void CloneSheet(ExcelPackage excel, ExcelWorksheet originalExcel, string sheetName, int newIndex)
            => excel.Workbook.Worksheets.Add(newIndex == 0 ? sheetName : $"{sheetName}_{newIndex}", originalExcel);

        /// <summary>
        /// 移除表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="sheetName"></param>
        /// <param name="newIndex"></param>
        private void RmoveSheet(ExcelPackage excel, string sheetName, int newIndex)
            => excel.Workbook.Worksheets.Delete(newIndex == 0 ? sheetName : $"{sheetName}_{newIndex}");

        /// <summary>
        /// 获取表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="sheetName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private ExcelWorksheet GetSheet(ExcelPackage excel, string sheetName, int index)
            => excel.Workbook.Worksheets[index == 0 ? sheetName : $"{sheetName}_{index}"];
        #endregion

        #region New

        private List<PerformanceMonitorResult> GetPerformanceMonitorResults(TaskArgument argument)
            => argument.PerformanceParseResults.OrderBy(result => (result.LogTime, result.LineNumber)).ToList();

        private void ExportPerformanceMonitorResult(ExcelRange range, int rowId, PerformanceMonitorResult result)
        {
            range[rowId, 1].Value = result.IPAddress;
            range[rowId, 2].Value = result.RequestSendTime;
            range[rowId, 3].Value = result.RequestReceiveTime;
            range[rowId, 4].Value = result.ResponseSendTime;
            range[rowId, 5].Value = result.ResponseReceiveTime;
            range[rowId, 6].Value = result.Elapsed;
            range[rowId, 7].Value = result.RequestDelay.TotalMilliseconds;
            range[rowId, 8].Value = result.ResponseElapsed.TotalMilliseconds;
            range[rowId, 9].Value = result.ResponseDelay.TotalMilliseconds;
            range[rowId, 10].Value = result.RequestURI;
            range[rowId, 11].Value = result.MethodName;
            range[rowId, 12].Value = result.RequestStreamLength;
            range[rowId, 13].Value = result.ResponseStreamLength;
            range[rowId, 14].Value = result.Message;
            range[rowId, 15].Value = result.LogFile.RelativePath;
            range[rowId, 16].Value = result.LineNumber;
        }

        private List<(int ExecuteID, TerminalAnalysisResult Result)> GetSQLAnalysisResults(TaskArgument argument)
            => argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.SQL || result.MonitorItem.SheetName == FixedDatas.SQL_SHEET_NAME)
                .Select(result => (1, result))
                .ToList();

        private void ExportSQLAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem.Name;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.DATABASE, out object database))
            {
                range[rowId, 2].Value = database;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.HASH, out object hash))
            {
                range[rowId, 3].Value = hash;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.RESULT_COUNT, out object count))
            {
                range[rowId, 4].Value = count;
            }

            range[rowId, 5].Value = result.ElapsedMillisecond;
            range[rowId, 6].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 7].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 8].Value = result.LogFile.RelativePath;
            range[rowId, 9].Value = result.LineNumber;

            if (result.AnalysisDatas.TryGetValue(FixedDatas.PARAMS, out object param))
            {
                range[rowId, 10].Value = param;
            }
        }
        #endregion

        #region 导出通用表

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
                Rectangle sheetRectangle = new Rectangle(1, 2, 9, FixedDatas.ExcelMaxRowCount);
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
                .Select(monitor => monitor.SheetName).Distinct()
                .Where(sheetName => !SpecialSheetNames.Contains(sheetName)))
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets[sheetName];
                if (worksheet == null)
                {
                    this.Tracer?.WriteLine($">未发现名称为 {sheetName} 的数据表，自动创建 ...");
                    try
                    {
                        worksheet = this.CreateCommonResultSheet(excel, sheetName);
                    }
                    catch (Exception ex)
                    {
                        this.Tracer?.WriteLine($"<创建表 {sheetName} 失败，将跳过导出此表的分析结果：{ex.Message}");
                        continue;
                    }
                }

                ExcelWorksheetPackage newPackage = this.CreateCommonWorksheetPackage(
                    worksheet,
                    argument.MonitorContainerRoot.GetTerminalMonitorItems()
                        .Where(monitor => monitor.SheetName == sheetName)
                        .Sum(monitor => monitor.AnalysisResults.Count));

                commonWorksheets.Add(sheetName, newPackage);
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
        /// 创建通用数据表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="sheetName"></param>
        private ExcelWorksheet CreateCommonResultSheet(ExcelPackage excel, string sheetName)
        {
            try
            {
                ExcelWorksheet sourceSheet = excel.Workbook.Worksheets[FixedDatas.ExcelSourceSheetName];
                if (sourceSheet == null)
                {
                    throw new Exception($"不存在 {FixedDatas.ExcelSourceSheetName} 表，无法创建通用表。");
                }

                excel.Workbook.Worksheets.Add(sheetName, sourceSheet);

                return excel.Workbook.Worksheets[sheetName] ?? throw new Exception($"创建 {sheetName} 表后，返回为 null");
            }
            catch
            {
                throw;
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
        #endregion

        #region 导出保留表

        /// <summary>
        /// 导出限额检查结果
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportLimitCheck(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.LIMIT_CHECK_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.LIMIT_CHECK_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.LIMIT_CHECK_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 12, FixedDatas.ExcelMaxRowCount);
                using (ExcelRange range = sheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top, executeID = 0;
                    object analysisData = null;

                    // 输出监视规则树
                    foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
                    {
                        // 每个分析结果根节点使执行序号自增
                        executeID++;

                        // 遍历根节点及所有子节点输出分析结果数据
                        foreach (TerminalAnalysisResult analysisResult in resultRoot.GetAnalysisResultWithSelf()
                            .Where(result =>
                                result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.LimitCheckAsync ||
                                result.MonitorItem.SheetName == FixedDatas.LIMIT_CHECK_SHEET_NAME))
                        {
                            range[rowID, 1].Value = analysisResult.MonitorItem.PrefixName;
                            range[rowID, 2].Value = analysisResult.Version;
                            range[rowID, 3].Value = executeID;
                            range[rowID, 4].Value = analysisResult.ElapsedMillisecond;
                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.SESSION_ID, out analysisData))
                            {
                                range[rowID, 5].Value = analysisData;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.USER_CODE, out analysisData))
                            {
                                range[rowID, 6].Value = analysisData;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.RESULT_COUNT, out analysisData))
                            {
                                range[rowID, 7].Value = analysisData;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.ROLE_COUNT, out analysisData))
                            {
                                range[rowID, 8].Value = analysisData;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.PRE_COUNT, out analysisData))
                            {
                                range[rowID, 9].Value = analysisData;
                            }

                            if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.PRO_COUNT, out analysisData))
                            {
                                range[rowID, 10].Value = analysisData;
                            }

                            range[rowID, 11].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 12].Value = analysisResult.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            range[rowID, 13].Value = analysisResult.LogFile.RelativePath;
                            range[rowID, 14].Value = analysisResult.LineNumber;

                            rowID++;
                        }
                    }
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
                Rectangle rectangle = new Rectangle(1, 2, 12, FixedDatas.ExcelMaxRowCount);
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
                Rectangle rectangle = new Rectangle(1, 2, 11, FixedDatas.ExcelMaxRowCount);
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
                Rectangle rectangle = new Rectangle(1, 2, 11, FixedDatas.ExcelMaxRowCount);
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
                Rectangle rectangle = new Rectangle(1, 2, 11, FixedDatas.ExcelMaxRowCount);
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
                Rectangle memoryRectangle = new Rectangle(1, 2, 5, FixedDatas.ExcelMaxRowCount);
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
                        memoryRange[rowID, 5].Value = result.AnalysisDatas.TryGetValue(FixedDatas.MEMORY_CONSUMED, out object memory) ? memory : 0.0;
                        memoryRange[rowID, 6].Value = result.AnalysisDatas.TryGetValue(FixedDatas.CPU_CONSUMED, out object cpu) ? cpu : 0.0;

                        rowID++;
                    }
                }
            }
        }

        /// <summary>
        /// 导出客户端消息数据表
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="argument"></param>
        public void ExportClientMessageSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet memoryDataSheet = excel.Workbook.Worksheets[FixedDatas.CLIENT_MESSAGE_SHEET_NAME];
            if (memoryDataSheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.CLIENT_MESSAGE_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.CLIENT_MESSAGE_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 5, FixedDatas.ExcelMaxRowCount);
                using (ExcelRange range = memoryDataSheet.Cells[rectangle.Top, rectangle.Left, rectangle.Bottom - 1, rectangle.Right - 1])
                {
                    int rowID = rectangle.Top;
                    foreach (var analysisResult in argument.TerminalAnalysisResults.Where(result => result.MonitorItem.SheetName == FixedDatas.CLIENT_MESSAGE_SHEET_NAME))
                    {
                        range[rowID, 1].Value = analysisResult.MonitorItem.PrefixName;
                        range[rowID, 2].Value = analysisResult.Version;
                        if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.MESSAGE_CODE, out object code))
                        {
                            range[rowID, 3].Value = code;
                        }

                        if (analysisResult.AnalysisDatas.TryGetValue(FixedDatas.MESSAGE_TABLE, out object table))
                        {
                            range[rowID, 4].Value = table;
                        }

                        range[rowID, 5].Value = analysisResult.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
                        range[rowID, 6].Value = analysisResult.LogFile.RelativePath;
                        range[rowID, 7].Value = analysisResult.LineNumber;

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
        public void ExportPerformanceAnalysiserSheet(ExcelPackage excel, TaskArgument argument)
        {
            ExcelWorksheet sheet = excel.Workbook.Worksheets[FixedDatas.PERFORMANCE_Analysiser_SHEET_NAME];
            if (sheet == null)
            {
                this.Tracer?.WriteLine($"未发现 {FixedDatas.PERFORMANCE_Analysiser_SHEET_NAME} 数据表，写入失败！");
            }
            else
            {
                this.Tracer?.WriteLine($"正在写入 {FixedDatas.PERFORMANCE_Analysiser_SHEET_NAME} 表数据 ...");
                Rectangle rectangle = new Rectangle(1, 2, 10, FixedDatas.ExcelMaxRowCount);
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
                Rectangle rectangle = new Rectangle(1, 2, 13, FixedDatas.ExcelMaxRowCount);
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
        #endregion
    }
}
