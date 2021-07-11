#undef Development

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using LogFactory.BIZ.Analysiser;
using LogFactory.BIZ.Exporter;
using LogFactory.BIZ.FileFinder;
using LogFactory.BIZ.Parser;
using LogFactory.Model;
using LogFactory.Model.Factory;
using LogFactory.Model.Fixed;
using LogFactory.Model.LogFile;
using LogFactory.Model.Monitor;
using LogFactory.Utils;
using LogFactory.Utils.Extensions;
using LogFactory.Utils.Trace;

// 在 VS 内通过 [按键:Alt+F2] 或 [菜单:(调试|分析)>性能探查器] 打开 [性能探查器] 分析方法或对象CPU或内存的性能影响

namespace LogFactory
{
    /// <summary>
    /// Program
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 全局任务参数
        /// </summary>
        public static volatile TaskArgument UnityTaskArgument = null;

        /// <summary>
        /// 全局追踪器
        /// </summary>
        public static volatile ITracer UnityTracer = new ConsoleTracer();

        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="args">启动参数</param>
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.Title = $"日志分析工具 - {Application.ProductVersion}";
            UnityTracer.WriteLine($"{Console.Title} 已启动...");
            UnityTracer.WriteLine($"启动参数：\n————————\n\t{string.Join("\n\t", args)}\n————————");

            UnityTracer.WriteLine("开始创建任务参数对象...");
            CreateTaskArgument(args);

            if (UnityTaskArgument?.IncludeSystemInfo ?? false)
            {
                UnityTracer.WriteLine("开始获取系统信息...");
                GetSystemInfo();
            }

            UnityTracer.WriteLine("准备监视规则XML文件存储目录：{0}", ConfigHelper.MonitorDirectory);
            CheckDirectory(ConfigHelper.MonitorDirectory);

            UnityTracer.WriteLine("准备日志报告导出目录：{0}", ConfigHelper.ReportExportDirectory);
            CheckDirectory(ConfigHelper.ReportExportDirectory);

            UnityTracer.WriteLine("准备报告导出模板目录：{0}", ConfigHelper.ReportTempletDirectory);
            CheckDirectory(ConfigHelper.ReportTempletDirectory);

            UnityTracer.WriteLine("开始反序列化匹配的监视规则对象...");
            GetMonitorItems(ConfigHelper.MonitorDirectory);
            ShowMonitorItems();

            UnityTracer.WriteLine("开始获取任务相关日志文件...");
            GetTaskLogFiles(UnityTaskArgument.LogDirectory, UnityTaskArgument);
            ShowLogFiles();

            UnityTracer.WriteLine("开始解析日志文件...");
            if (UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().Count() > 0)
            {
                ParseClientTerminalLog();
                ParseServerTerminalLog();
                ParseSQLLog();
            }

            if (UnityTaskArgument.MonitorContainerRoot.GetPerformanceMonitorItems().Count() > 0)
            {
                ParseClientPerformanceLog();
                ParseServerPerformanceLog();
                ParseOldPerformanceLog();

                ShowPerformanceParseResult();
            }

            ShowParseResult();
            GC.Collect();

            UnityTracer.WriteLine("开始分析日志解析结果...");
            AnalysisLog();
            ShowAnalysisResult();
            GC.Collect();

            UnityTaskArgument.TaskFinishTime = DateTime.Now;
            TryToExportLogReport();
            SaveTaskArgumentToXML(UnityTaskArgument.DeepClone());
            GC.Collect();

#if DEBUG
            // 调试助手
            new DebugHelper(UnityTracer, false).ActiveDebugFunction(UnityTaskArgument);
#endif

            Exit(0);
        }
        #region 准备任务及相关数据

        /// <summary>
        /// 获取任务相关日志文件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="argument"></param>
        private static void GetTaskLogFiles(string directory, TaskArgument argument)
        {
            TaskFileFinderBase logFinder = new LogFileFinder();

            try
            {
                if (!Directory.Exists(directory))
                {
                    throw new DirectoryNotFoundException($"不存在的日志文件目录：{directory}");
                }

                logFinder.GetTaskObject<object>(UnityTaskArgument.LogDirectory, UnityTaskArgument);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"获取任务相关日志文件失败：{ex.Message}");
                Exit(4);
            }
        }

        /// <summary>
        /// 获取监视规则对象
        /// </summary>
        /// <param name="directory"></param>
        private static void GetMonitorItems(string directory)
        {
            TaskFileFinderBase monitorFinder = new MonitorFileFinder();
            MonitorContainer monitorContainer = null;

            try
            {
                monitorContainer = monitorFinder.GetTaskObject<MonitorContainer>(directory, UnityTaskArgument);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"获取任务相关监视规则失败：{ex.Message}");
                Exit(4);
            }

            if (monitorContainer != null)
            {
                UnityTaskArgument.MonitorContainerRoot = monitorContainer;
            }
            else
            {
                UnityTracer.WriteLine($"获取监视规则容器对象为空！");
                Exit(4);
            }
        }

        /// <summary>
        /// 检查工作目录
        /// </summary>
        /// <param name="directory">工作目录</param>
        private static void CheckDirectory(string directory)
        {
            try
            {
                IOUtils.PrepareDirectory(directory);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"准备工作目录失败：{ex.Message}");
                Exit(2);
            }

            UnityTracer.WriteLine("准备工作目录成功");
        }

        /// <summary>
        /// 创建任务参数
        /// </summary>
        /// <param name="args"></param>
        private static void CreateTaskArgument(string[] args)
        {
            if (ConfigHelper.UseGUITaskFactory)
            {
                GUICreateTask(null);
            }
            else
            {
                ArgsCreateTask(args);
            }

            if (UnityTaskArgument == null)
            {
                throw new ArgumentException("创建任务失败！");
            }

            UnityTracer.WriteLine("创建任务参数对象成功：\n————————\n{0}\n————————", UnityTaskArgument);
        }

        /// <summary>
        /// 通过命令行参数创建任务
        /// </summary>
        /// <param name="args"></param>
        private static void ArgsCreateTask(string[] args)
        {
            try
            {
                UnityTaskArgument = ArgsTaskArgumentFactory.Intance.CreateTaskArgument(args);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"创建任务参数对象失败：{ex.Message}");
                UnityTracer.WriteLine(ArgsTaskArgumentFactory.Intance.Usage);

                UnityTracer.WriteLine($"正在使用 GUI 创建任务：");
                GUICreateTask(ex.Data["TaskArgument"] as TaskArgument);
            }
        }

        /// <summary>
        /// 通过GUI创建任务
        /// </summary>
        /// <param name="argument"></param>
        private static void GUICreateTask(TaskArgument argument)
        {
            try
            {
                UnityTaskArgument = GUITaskArgumentFactory.Intance.CreateTaskArgument(argument);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"创建任务参数对象失败：{ex.Message}");
                Exit(1);
            }
        }

        /// <summary>
        /// 获取系统信息
        /// </summary>
        private static void GetSystemInfo()
        {
            SystemInfo systemInfo = new SystemInfo();
            UnityTaskArgument.SystemInfo = systemInfo;
            ShowSystemInfo();
        }

        #endregion

        #region 解析日志

        /// <summary>
        /// 解析客户端日志
        /// </summary>
        private static void ParseClientTerminalLog()
        {
            if (UnityTaskArgument.TerminalLogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Client) > 0)
            {
                UnityTracer.WriteLine("开始解析 [客户端] 日志文件...");

                ILogParser clientLogParser = new ClientTerminalParser(UnityTracer);
                clientLogParser.Parse(UnityTaskArgument);

                List<TerminalLogFile> clientLogFiles = UnityTaskArgument.TerminalLogFiles
                    .Where(logFile => logFile.LogFileType == LogFileTypes.Client && logFile.MonitorResults.Count > 0).ToList();

                UnityTracer.WriteLine(
                    "[客户端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    clientLogFiles.Count,
                    UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Client)),
                    clientLogFiles.Sum(logFile => logFile.MonitorResults.Count));
            }
        }

        /// <summary>
        /// 解析服务端日志
        /// </summary>
        private static void ParseServerTerminalLog()
        {
            if (UnityTaskArgument.TerminalLogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Server) > 0)
            {
                UnityTracer.WriteLine("开始解析 [服务端] 日志文件...\n————————");

                ILogParser serverLogParser = new ServerTerminalParser(UnityTracer);
                serverLogParser.Parse(UnityTaskArgument);

                List<TerminalLogFile> serverLogFiles = UnityTaskArgument.TerminalLogFiles
                    .Where(logFile => logFile.LogFileType == LogFileTypes.Server && logFile.MonitorResults.Count > 0).ToList();

                UnityTracer.WriteLine(
                    "[服务端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    serverLogFiles.Count,
                    UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Server)),
                    serverLogFiles.Sum(logFile => logFile.MonitorResults.Count));
            }
        }

        /// <summary>
        /// 解析服务端Performance日志
        /// </summary>
        private static void ParseServerPerformanceLog()
        {
            if (UnityTaskArgument.PerformanceLogFiles.Count() > 0)
            {
                UnityTracer.WriteLine("开始解析 [服务端-Performance] 日志文件...\n————————");

                ILogParser performanceLogParser = new ServerPerformanceParser(UnityTracer);
                performanceLogParser.Parse(UnityTaskArgument);
            }
        }

        /// <summary>
        /// 解析SQL日志
        /// </summary>
        private static void ParseSQLLog()
        {
            if (UnityTaskArgument.TerminalLogFiles.Count(file => file.LogFileType == LogFileTypes.Server && file.LogLevel == LogLevels.SQL) > 0)
            {
                UnityTracer.WriteLine("开始解析 [SQL] 日志文件...\n————————");

                ILogParser sqlParser = new ServerSQLTerminalParser(UnityTracer);
                sqlParser.Parse(UnityTaskArgument);
            }
        }

        /// <summary>
        /// 解析旧版Performance日志
        /// </summary>
        private static void ParseOldPerformanceLog()
        {
            if (UnityTaskArgument.PerformanceLogFiles.Count() > 0)
            {
                UnityTracer.WriteLine("开始解析 [旧版-Performance] 日志文件...\n————————");

                ILogParser performanceLogParser = new OldPerformanceParser(UnityTracer);
                performanceLogParser.Parse(UnityTaskArgument);
            }
        }

        /// <summary>
        /// 解析客户端Performance日志
        /// </summary>
        private static void ParseClientPerformanceLog()
        {
            if (UnityTaskArgument.PerformanceLogFiles.Count() > 0)
            {
                UnityTracer.WriteLine("开始解析 [客户端-Performance] 日志文件...\n————————");

                ILogParser performanceLogParser = new ClientPerformanceParser(UnityTracer);
                performanceLogParser.Parse(UnityTaskArgument);
            }
        }
        #endregion

        #region 分析日志

        /// <summary>
        /// 分析日志解析结果
        /// </summary>
        private static void AnalysisLog()
        {
            // 分析结果前先清空分析结果
            UnityTaskArgument.TerminalAnalysisResults.Clear();
            UnityTaskArgument.TerminalLogFiles.ForEach(logFile => logFile.AnalysisResults.Clear());
            UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().ToList().ForEach(monitor => monitor.AnalysisResults.Clear());

            UnityTaskArgument.PerformanceAnalysisResults.Clear();
            UnityTaskArgument.PerformanceLogFiles.ForEach(logFile => logFile.AnalysisResults.Clear());
            UnityTaskArgument.MonitorContainerRoot.GetPerformanceMonitorItems().ToList().ForEach(monitor => monitor.AnalysisResults.Clear());

            LogAnalysiserHost logAnalysiserHost = new LogAnalysiserHost(UnityTracer);
            logAnalysiserHost.Analysis(UnityTaskArgument);
        }

        #endregion

        #region 导出报告

        /// <summary>
        /// 将任务储存为XML文件
        /// </summary>
        /// <param name="argument">任务参数对象</param>
        private static void SaveTaskArgumentToXML(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            try
            {
                string taskXMLPath = GetXMLFilePath(argument);
                string xmlContent = argument.SerializeToXML();
                File.WriteAllText(taskXMLPath, xmlContent);
                UnityTracer.WriteLine($"任务对象存储到XML成功：{taskXMLPath}");

                // Process.Start("notepad.exe", taskXMLPath);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"任务对象存储到XML失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取任务导出XML文件路径
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static string GetXMLFilePath(TaskArgument argument)
        {
            return $@"{ConfigHelper.ReportExportDirectory}\任务数据转储_{argument.MonitorContainerRoot.Name}_{argument.LogStartTime?.ToString("yyyyMMddHHmmss")}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xml";
        }

        /// <summary>
        /// 尝试导出日志报告
        /// </summary>
        private static void TryToExportLogReport()
        {
            string reportPath = string.Empty;
            string chartsPath = string.Empty;
            bool reportSuccess = false;
            bool chartsSuccess = false;

#if Development
            reportSuccess = true;
#endif

            // 当导出失败且用户同意重试时重复导出，并在失败时再次询问用户
            do
            {
                try
                {
                    if (!reportSuccess)
                    {
                        reportPath = GetReportFilePath(UnityTaskArgument);
                        UnityTracer.WriteLine("开始导出日志报告...");
                        ExportLogReport(reportPath);
                        reportSuccess = true;

                        // 记录日志报告导出路径
                        UnityTaskArgument.LastReportPath = reportPath;
                        UnityTracer.WriteLine($"日志报告导出成功=> {reportPath}");
                    }

                    if (!chartsSuccess)
                    {
                        chartsPath = GetChartsReportFilePath(UnityTaskArgument);

                        UnityTracer.WriteLine("开始导出图表报告...");
                        ExportChartsReport(chartsPath);
                        chartsSuccess = true;
                        UnityTracer.WriteLine($"图表报告导出成功=> {chartsPath}");
                    }
                }
                catch (Exception ex)
                {
                    UnityTracer.WriteLine($"导出日志报告失败：{ex.Message}");
                    UnityTracer.WriteLine("是否重试？(请输入： Y / N )");
                }
            }
            while (!(reportSuccess && chartsSuccess) &&
                      !UnityTaskArgument.AutoExit &&
                      Console.ReadLine().Trim().ToUpper() == "Y");

            if (UnityTaskArgument.AutoOpenReport)
            {
                if (reportSuccess && File.Exists(reportPath))
                {
                    Process.Start(reportPath);
                }

                if (chartsSuccess && File.Exists(chartsPath))
                {
                    Process.Start(chartsPath);
                }
            }
        }

        /// <summary>
        /// 导出图表报告
        /// </summary>
        /// <param name="reportPath">报告路径</param>
        private static void ExportChartsReport(string reportPath)
        {
            ILogReportExporter chartsExporter = new ChartsReportExporter(UnityTracer);

            try
            {
                chartsExporter.ExportReport(reportPath, UnityTaskArgument);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 导出日志报告
        /// </summary>
        /// <param name="reportPath">报告路径</param>
        private static void ExportLogReport(string reportPath)
        {
            ILogReportExporter reportExporter = null;

            switch (UnityTaskArgument.ReportMode)
            {
                case ReportModes.Excel:
                    {
                        reportExporter = new ExcelLogReportExporter(UnityTracer);
                        break;
                    }

                case ReportModes.HTML:
                    {
                        reportExporter = new HTMLLogReportExporter();
                        break;
                    }

                case ReportModes.Word:
                    {
                        throw new NotImplementedException($"暂未实现 {UnityTaskArgument.ReportMode} 报告格式对应的报告导出器，请等待程序开发...");
                    }

                default:
                    {
                        throw new InvalidOperationException($"未找到 {UnityTaskArgument.ReportMode} 报告格式对应的报告导出器！");
                    }
            }

            try
            {
                reportExporter.ExportReport(reportPath, UnityTaskArgument);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取导出报告文件路径
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static string GetReportFilePath(TaskArgument argument)
        {
            return $@"{ConfigHelper.ReportExportDirectory}\导出报告_{Path.GetFileName(argument.LogDirectory)}_{argument.MonitorContainerRoot.Name}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.{argument.ReportMode.GetAmbientValue()}";
        }

        /// <summary>
        /// 获取图表报告文件路径
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static string GetChartsReportFilePath(TaskArgument argument)
        {
            return $@"{ConfigHelper.ReportExportDirectory}\图表报告_{Path.GetFileName(argument.LogDirectory)}_{argument.MonitorContainerRoot.Name}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.html";
        }
        #endregion

        #region 系统

        /// <summary>
        /// 程序结束
        /// </summary>
        /// <param name="code">程序退出代码 (0: 正常退出)</param>
        private static void Exit(int code)
        {
            // 记录任务完成时间
            if (UnityTaskArgument != null &&
                (UnityTaskArgument.TaskFinishTime == DateTime.MinValue ||
                 UnityTaskArgument.TaskFinishTime == null))
            {
                UnityTaskArgument.TaskFinishTime = DateTime.Now;
            }

            GC.Collect();

            Console.WriteLine("\n————————");
            ShowTaskDuration();
            Console.WriteLine("————————");

            if (UnityTaskArgument == null || !UnityTaskArgument.AutoExit)
            {
                Console.WriteLine("按任意键退出此程序... (￣▽￣)／");
                Console.Read();
            }

            Environment.Exit(code);
        }

        /// <summary>
        /// 处理当前域未捕捉异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception unhandledException))
            {
                return;
            }

            if (unhandledException is OutOfMemoryException)
            {
                GC.KeepAlive(UnityTaskArgument);
                GC.Collect();

                if (!e.IsTerminating)
                {
                    return;
                }
            }

            string exceptionDescription = string.Format(
                "应用域内发现未被捕获的异常：(;′⌒`)\r\n" +
                "\t【异常类型】: {0}\r\n" +
                "\t【异常地址】: {1}\r\n" +
                "\t【出错方法】: {2}\r\n" +
                "\t【所在文件】: {3}\r\n" +
                "\t【异常信息】: {4}\r\n" +
                "\t【调用堆栈】: \r\n{5}\r\n" +
                "\t【即将终止】: {6}",
                unhandledException.GetType().ToString(),
                unhandledException.Source,
                unhandledException.TargetSite.Name,
                unhandledException.TargetSite.Module.FullyQualifiedName,
                unhandledException.Message,
                unhandledException.StackTrace,
                e.IsTerminating);

            Console.WriteLine("——————————————————");
            UnityTracer.WriteLine(exceptionDescription);
            Console.WriteLine("——————————————————");
            Exit(int.MaxValue);
        }

        #endregion

        #region 输出信息

        /// <summary>
        /// 显示任务耗时
        /// </summary>
        private static void ShowTaskDuration()
        {
            if (UnityTaskArgument != null)
            {
                TimeSpan duration = UnityTaskArgument.TaskFinishTime.Subtract(UnityTaskArgument.TaskStartTime);
                Console.WriteLine($"任务耗时：{duration.Hours}时 {duration.Minutes}分 {duration.Seconds}秒 {duration.Milliseconds}毫秒");
            }
        }

        /// <summary>
        /// 显示监视规则
        /// </summary>
        private static void ShowMonitorItems()
        {
            List<TerminalMonitorItem> terminalMonitors = UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().ToList();
            List<PerformanceMonitorItem> performanceMonitors = UnityTaskArgument.MonitorContainerRoot.GetPerformanceMonitorItems().ToList();
            int monitorCount = terminalMonitors.Count + performanceMonitors.Count;

            if (monitorCount == 0)
            {
                UnityTracer.WriteLine("未发现监视规则，程序即将退出");
                Exit(4);
            }

            UnityTracer.WriteLine($"发现 {monitorCount} 个任务相关监视规则对象：\n————————{(terminalMonitors.Count > 0 ? $"\n\tTerminal：{string.Join("、", terminalMonitors.Select(item => item.Name))}" : string.Empty)}{(performanceMonitors.Count > 0 ? $"\n\tPerformance：{string.Join("、", performanceMonitors.Select(item => item.Name))}" : string.Empty)}\n————————");
        }

        /// <summary>
        /// 显示日志文件信息
        /// </summary>
        private static void ShowLogFiles()
        {
            int fileCount = UnityTaskArgument.TerminalLogFiles.Count + UnityTaskArgument.PerformanceLogFiles.Count;
            if (fileCount == 0)
            {
                UnityTracer.WriteLine("未发现任务相关日志文件，程序将退出");
                Exit(3);
            }
            else
            {
                string terminalLogFileContent = string.Join("\n\t", UnityTaskArgument.TerminalLogFiles.Select(file => file.RelativePath));
                string performanceLogFileContent = string.Join("\n\t", UnityTaskArgument.PerformanceLogFiles.Select(file => file.RelativePath));

                if (!string.IsNullOrEmpty(terminalLogFileContent))
                {
                    terminalLogFileContent = $"\n\t{terminalLogFileContent}";
                }

                if (!string.IsNullOrEmpty(performanceLogFileContent))
                {
                    performanceLogFileContent = $"\n\t{performanceLogFileContent}";
                }

                UnityTracer.WriteLine($"发现 {fileCount} 个日志文件：\n————————{terminalLogFileContent}{performanceLogFileContent}\n————————");
            }
        }

        /// <summary>
        /// 显示解析结果
        /// </summary>
        private static void ShowParseResult()
        {
            UnityTracer.WriteLine(
                "所有日志文件解析完成：\n\t[共计] 在 {0} 个文件中发现：\n\t\t {1}\t 个监视规则的 {2}\t 个监视结果和\n\t\t {3}\t 个监视规则的 {4}\t 个Performance监视结果和\n\t\t {5}\t 个Performance解析结果\n————————",
                UnityTaskArgument.TerminalLogFiles.Count(file => file.MonitorResults.Count > 0) + UnityTaskArgument.PerformanceLogFiles.Count(file => file.MonitorResults.Count > 0),
                UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().Count(monitor => monitor.MonitorResults.Count > 0),
                UnityTaskArgument.TerminalMonitorResults.Count,
                UnityTaskArgument.MonitorContainerRoot.GetPerformanceMonitorItems().Count(monitor => monitor.MonitorResults.Count > 0),
                UnityTaskArgument.PerformanceMonitorResults.Count,
                UnityTaskArgument.PerformanceParseResults.Count);
        }

        /// <summary>
        /// 显示分析结果
        /// </summary>
        private static void ShowAnalysisResult()
        {
            UnityTracer.WriteLine(
                "日志解析结果分析完成：\n\t在 {0} 个文件中匹配到 {1} 个监视规则的 {2} 组分析结果\n————————",
                UnityTaskArgument.TerminalLogFiles.Count(file => file.AnalysisResults.Count > 0) + UnityTaskArgument.PerformanceLogFiles.Count(file => file.AnalysisResults.Count > 0),
                UnityTaskArgument.MonitorContainerRoot.GetTerminalMonitorItems().Count(monitor => monitor.AnalysisResults.Count > 0) + UnityTaskArgument.MonitorContainerRoot.GetPerformanceMonitorItems().Count(monitor => monitor.AnalysisResults.Count > 0),
                UnityTaskArgument.TerminalAnalysisResults.Count + UnityTaskArgument.PerformanceAnalysisResults.Count);

            /*
            UnityTrace.WriteLine("分析结果树：");

            UnityTaskArgument.AnalysisResultContainerRoot
                .GetTerminalAnalysisResults().ToList()
                .ForEach(result => Console.WriteLine(result));
            UnityTaskArgument.AnalysisResultContainerRoot
                .GetPerformanceAnalysisResults().ToList()
                .ForEach(result => Console.WriteLine(result));
             */
        }

        /// <summary>
        /// 显示Performance日志解析结果
        /// </summary>
        private static void ShowPerformanceParseResult()
        {
            UnityTracer.WriteLine(
                "[Performance] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个解析结果和 {2} 个监视结果\n————————",
                UnityTaskArgument.PerformanceLogFiles.Count(file => file.MonitorResults.Count > 0),
                UnityTaskArgument.PerformanceParseResults.Count,
                UnityTaskArgument.PerformanceMonitorResults.Count);
        }

        /// <summary>
        /// 输出系统信息
        /// </summary>
        private static void ShowSystemInfo()
        {
            UnityTracer.WriteLine($"获取系统信息完成：\n————————\n{UnityTaskArgument.SystemInfo?.ToString()}\n————————");
        }
        #endregion
    }
}
