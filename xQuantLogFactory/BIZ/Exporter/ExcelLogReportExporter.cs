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
            FixedDatas.PERFORMANCE_ANALYSISER_SHEET_NAME,
            FixedDatas.PERFORMANCE_PARSE_SHEET_NAME,
            FixedDatas.TRADE_CLEARING_SHEET_NAME,
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
                    this.ExportSheet(excel, FixedDatas.LIMIT_CHECK_SHEET_NAME, argument, this.GetLimitCheckAnalysisResults, this.ExportLimitCheckAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.CACHE_SHEET_NAME, argument, this.GetCacheAnalysisResults, this.ExportCacheAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.REPORT_SHEET_NAME, argument, this.GetReportAnalysisResults, this.ExportReportAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.FORM_SHEET_NAME, argument, this.GetFormAnalysisResults, this.ExportFormAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.CORE_SERVICE_SHEET_NAME, argument, this.GetCoreServiceAnalysisResults, this.ExportCoreServiceAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.MEMORY_SHEET_NAME, argument, this.GetMemoryAnalysisResults, this.ExportMemoryAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.CLIENT_MESSAGE_SHEET_NAME, argument, this.GetClientMessageAnalysisResults, this.ExportClientMessageAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.PERFORMANCE_ANALYSISER_SHEET_NAME, argument, this.GetPerformanceAnalysisResults, this.ExportPerformanceAnalysiserResult);
                    this.ExportSheet(excel, FixedDatas.TRADE_CLEARING_SHEET_NAME, argument, this.GetTradeClearingAnalysisResults, this.ExportTradeClearingAnalysiserResult);

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
        #region Performance 解析结果

        /// <summary>
        /// 筛选Performance解析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<PerformanceMonitorResult> GetPerformanceMonitorResults(TaskArgument argument)
            => argument.PerformanceParseResults.OrderBy(result => (result.LogTime, result.LineNumber)).ToList();

        /// <summary>
        /// 导出Performance解析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="result"></param>
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
        #endregion

        #region SQL分析结果

        /// <summary>
        /// 筛选SQL分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult Result)> GetSQLAnalysisResults(TaskArgument argument)
            => argument.TerminalAnalysisResults
                .Where(result => result.MonitorItem.DirectedAnalysiser == TerminalDirectedAnalysiserTypes.SQL || result.MonitorItem.SheetName == FixedDatas.SQL_SHEET_NAME)
                .Select(result => (1, result))
                .ToList();

        /// <summary>
        /// 导出SQL分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
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

        #region 限额检查分析结果

        /// <summary>
        /// 过滤限额检查分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetLimitCheckAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, TerminalAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Where(result =>
                        result.MonitorItem.GroupAnalysiser == TerminalGroupAnalysiserTypes.LimitCheckAsync ||
                        result.MonitorItem.SheetName == FixedDatas.LIMIT_CHECK_SHEET_NAME)
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出限额检查分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportLimitCheckAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem.PrefixName;
            range[rowId, 2].Value = result.Version;
            range[rowId, 3].Value = executeId;
            range[rowId, 4].Value = result.ElapsedMillisecond;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.SESSION_ID, out object analysisData))
            {
                range[rowId, 5].Value = analysisData;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.USER_CODE, out analysisData))
            {
                range[rowId, 6].Value = analysisData;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.RESULT_COUNT, out analysisData))
            {
                range[rowId, 7].Value = analysisData;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.ROLE_COUNT, out analysisData))
            {
                range[rowId, 8].Value = analysisData;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.PRE_COUNT, out analysisData))
            {
                range[rowId, 9].Value = analysisData;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.PRO_COUNT, out analysisData))
            {
                range[rowId, 10].Value = analysisData;
            }

            range[rowId, 11].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 12].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 13].Value = result.LogFile.RelativePath;
            range[rowId, 14].Value = result.LineNumber;
        }
        #endregion

        #region 缓存统计分析结果

        /// <summary>
        /// 过滤缓存统计分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetCacheAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, TerminalAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Where(result => result.MonitorItem.SheetName == FixedDatas.CACHE_SHEET_NAME)
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出缓存统计分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportCacheAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem.PrefixName;
            range[rowId, 2].Value = result.MonitorItem.ParentMonitorItem?.Name;
            range[rowId, 3].Value = result.Version;
            range[rowId, 4].Value = executeId;
            range[rowId, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond : 0.0;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.RESOURCE_NAME, out object name))
            {
                range[rowId, 6].Value = name;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.COUNT, out object count))
            {
                range[rowId, 7].Value = count;
            }

            range[rowId, 8].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 9].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 10].Value = result.LogFile.RelativePath;
            range[rowId, 11].Value = result.LineNumber;
        }
        #endregion

        #region 报表分析结果

        /// <summary>
        /// 过滤报表分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetReportAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, TerminalAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Where(result => result.MonitorItem.SheetName == FixedDatas.REPORT_SHEET_NAME)
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出报表分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportReportAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem.PrefixName;
            range[rowId, 2].Value = result.MonitorItem.ParentMonitorItem?.Name;
            range[rowId, 3].Value = result.Version;
            range[rowId, 4].Value = executeId;
            range[rowId, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond : 0.0;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.REPORT_CODE, out object code))
            {
                range[rowId, 6].Value = code;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.REPORT_NAME, out object name))
            {
                range[rowId, 7].Value = name;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.QUERY_PARAM, out object param))
            {
                range[rowId, 8].Value = param;
            }

            range[rowId, 9].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 10].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 11].Value = result.LogFile.RelativePath;
            range[rowId, 12].Value = result.LineNumber;
        }
        #endregion

        #region 窗体分析结果

        /// <summary>
        /// 过滤窗体分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetFormAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, TerminalAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Where(result => result.MonitorItem.SheetName == FixedDatas.FORM_SHEET_NAME)
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出窗体分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportFormAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem.PrefixName;
            range[rowId, 2].Value = result.MonitorItem.ParentMonitorItem?.Name;
            range[rowId, 3].Value = result.Version;
            range[rowId, 4].Value = executeId;
            range[rowId, 5].Value = result.IsIntactGroup() ? result.ElapsedMillisecond : 0.0;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.MODULE_CODE, out object code))
            {
                range[rowId, 6].Value = code;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.FORM_NAME, out object name))
            {
                range[rowId, 7].Value = name;
            }

            range[rowId, 8].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 9].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 10].Value = result.LogFile.RelativePath;
            range[rowId, 11].Value = result.LineNumber;
        }
        #endregion

        #region Core服务分析结果

        /// <summary>
        /// 过滤Core服务分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetCoreServiceAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, TerminalAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Where(result => result.MonitorItem.SheetName == FixedDatas.CORE_SERVICE_SHEET_NAME)
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出Core服务分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportCoreServiceAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.Version;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.CORE_SERVICE_NAME, out object service))
            {
                range[rowId, 2].Value = service;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.EXECUTE_INDEX, out object index))
            {
                range[rowId, 3].Value = index;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.TRIGGER, out object trigger))
            {
                range[rowId, 4].Value = trigger;
            }

            range[rowId, 5].Value = result.ElapsedMillisecond;
            range[rowId, 6].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 7].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 8].Value = result.LogFile?.RelativePath;
            range[rowId, 9].Value = result.StartMonitorResult?.LineNumber;
            range[rowId, 10].Value = result.FinishMonitorResult?.LineNumber;
        }
        #endregion

        #region 内存分析结果

        /// <summary>
        /// 过滤内存分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetMemoryAnalysisResults(TaskArgument argument)
            => argument.TerminalAnalysisResults
                    .Where(result => result.MonitorItem.Memory)
                    .Distinct(new TerminalAnalysisResultEqualityComparer<TerminalAnalysisResult>())
                    .Select(result => (1, result))
                    .ToList();

        /// <summary>
        /// 导出内存分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportMemoryAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem?.Name;
            range[rowId, 2].Value = result.Version;
            range[rowId, 3].Value = result.Client;
            range[rowId, 4].Value = result.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 5].Value = result.AnalysisDatas.TryGetValue(FixedDatas.MEMORY_CONSUMED, out object memory) ? memory : 0.0;
            range[rowId, 6].Value = result.AnalysisDatas.TryGetValue(FixedDatas.CPU_CONSUMED, out object cpu) ? cpu : 0.0;
        }
        #endregion

        #region 客户端消息分析结果

        /// <summary>
        /// 过滤客户端消息分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetClientMessageAnalysisResults(TaskArgument argument)
            => argument.TerminalAnalysisResults
                    .Where(result => result.MonitorItem.SheetName == FixedDatas.CLIENT_MESSAGE_SHEET_NAME)
                    .Select(result => (1, result))
                    .ToList();

        /// <summary>
        /// 导出客户端消息分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportClientMessageAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem.PrefixName;
            range[rowId, 2].Value = result.Version;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.MESSAGE_CODE, out object code))
            {
                range[rowId, 3].Value = code;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.MESSAGE_TABLE, out object table))
            {
                range[rowId, 4].Value = table;
            }

            range[rowId, 5].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 6].Value = result.LogFile.RelativePath;
            range[rowId, 7].Value = result.LineNumber;
        }
        #endregion

        #region Performance分析结果

        /// <summary>
        /// 过滤Performance分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, PerformanceAnalysisResult result)> GetPerformanceAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, PerformanceAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.PerformanceAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出Performance分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportPerformanceAnalysiserResult(ExcelRange range, int rowId, int executeId, PerformanceAnalysisResult result)
        {
            range[rowId, 1].Value = result.IPAddress;
            range[rowId, 2].Value = result.UserCode;
            range[rowId, 3].Value = result.MonitorItem.PrefixName;
            range[rowId, 4].Value = result.MonitorItem.ParentMonitorItem?.Name;
            range[rowId, 5].Value = executeId;
            range[rowId, 6].Value = result.IsIntactGroup() ? result.ElapsedMillisecond : 0.0;
            range[rowId, 7].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 8].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 9].Value = result.LogFile.RelativePath;
            range[rowId, 10].Value = result.LineNumber;
        }
        #endregion

        #region 交易清算分析结果

        /// <summary>
        /// 过滤交易清算分析结果
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private List<(int ExecuteID, TerminalAnalysisResult result)> GetTradeClearingAnalysisResults(TaskArgument argument)
        {
            int executeID = 0;
            var results = new List<(int ExecuteID, TerminalAnalysisResult result)>();
            foreach (var resultRoot in argument.AnalysisResultContainerRoot.TerminalAnalysisResultRoots)
            {
                executeID++;

                results.AddRange(resultRoot.GetAnalysisResultWithSelf()
                    .Where(result => result.MonitorItem.SheetName == FixedDatas.TRADE_CLEARING_SHEET_NAME)
                    .Select(result => (executeID, result)));
            }

            return results;
        }

        /// <summary>
        /// 导出交易清算分析结果
        /// </summary>
        /// <param name="range"></param>
        /// <param name="rowId"></param>
        /// <param name="executeId"></param>
        /// <param name="result"></param>
        private void ExportTradeClearingAnalysiserResult(ExcelRange range, int rowId, int executeId, TerminalAnalysisResult result)
        {
            range[rowId, 1].Value = result.MonitorItem?.PrefixName;
            range[rowId, 2].Value = result.MonitorItem?.ParentMonitorItem?.Name;
            range[rowId, 3].Value = result.Version;
            range[rowId, 4].Value = executeId;
            range[rowId, 5].Value = result.ElapsedMillisecond;
            if (result.AnalysisDatas.TryGetValue(FixedDatas.QSRQ, out object qsrq))
            {
                range[rowId, 6].Value = qsrq;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.DQZH, out object dqzh))
            {
                range[rowId, 7].Value = dqzh;
            }

            if (result.AnalysisDatas.TryGetValue(FixedDatas.WBZH, out object wbzh))
            {
                range[rowId, 8].Value = wbzh;
            }

            range[rowId, 9].Value = result.StartMonitorResult?.LogTime; // .ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 10].Value = result.FinishMonitorResult?.LogTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            range[rowId, 11].Value = result.LogFile?.RelativePath;

            range[rowId, 12].Value = result.StartMonitorResult?.LineNumber;
            range[rowId, 13].Value = result.FinishMonitorResult?.LineNumber;
        }
        #endregion
        #endregion

        #region 导出通用表

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
             * 思路：同初始化分析结果树算法，遍历分析结果容器的根节点（仅在容器根节点遍历时更新执行序号），对每个根节点当做树根遍历，将所有子节点分表导出
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
    }
}
