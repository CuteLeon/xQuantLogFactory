using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using xQuantLogFactory.BIZ.Analysiser;
using xQuantLogFactory.BIZ.Exporter;
using xQuantLogFactory.BIZ.FileFinder;
using xQuantLogFactory.BIZ.Parser;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Factory;
using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Extensions;
using xQuantLogFactory.Utils.Trace;

// 在 VS 内通过 [按键:Alt+F2] 或 [菜单:(调试|分析)>性能探查器] 打开 [性能探查器] 分析方法或对象CPU或内存的性能影响

// TODO: [全局任务] 移除和排除 using
// TODO: [全局任务] 编写单元测试
namespace xQuantLogFactory
{
    // TODO: 实体类内的列表类型改为线程安全类型是否可以代替Monitor.CANO的作用？

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

            Console.Title = $"xQuant 日志分析工具 - {Application.ProductVersion}";
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
            UnityTracer.WriteLine($"发现 {UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Count()} 个任务相关监视规则对象：\n————————\n{string.Join("、", UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Select(item => item.Name))}\n————————");

            UnityTracer.WriteLine("开始获取任务相关日志文件...");
            GetTaskLogFiles(UnityTaskArgument.LogDirectory);

            UnityTracer.WriteLine("开始解析日志文件...");

            // 未发现监视规则对象，不解析客户端和服务端日志文件
            if (UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Count() > 0)
            {
                ParseClientLog();
                ParseServerLog();
            }

            ParseMiddlewareLog();
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
            new DebugHelper(UnityTracer).ActiveDebugFunction(UnityTaskArgument);
#endif

            Exit(0);
        }

        #region 准备任务及相关数据

        /// <summary>
        /// 获取任务相关日志文件
        /// </summary>
        /// <param name="directory"></param>
        private static void GetTaskLogFiles(string directory)
        {
            TaskFileFinderBase logFinder = new LogFileFinder();
            IEnumerable<LogFile> logFiles = null;

            try
            {
                if (!Directory.Exists(directory))
                {
                    throw new DirectoryNotFoundException($"不存在的日志文件目录：{directory}");
                }

                logFiles = logFinder.GetTaskObject<IEnumerable<LogFile>>(UnityTaskArgument.LogDirectory, UnityTaskArgument);
            }
            catch (Exception ex)
            {
                UnityTracer.WriteLine($"获取任务相关日志文件失败：{ex.Message}");
                Exit(4);
            }

            if (logFiles.Count() > 0)
            {
                UnityTaskArgument.LogFiles.AddRange(logFiles);
            }

            if (UnityTaskArgument.LogFiles.Count == 0)
            {
                UnityTracer.WriteLine("未发现任务相关日志文件，程序将退出");
                Exit(3);
            }
            else
            {
                UnityTracer.WriteLine($"发现 {UnityTaskArgument.LogFiles.Count} 个日志文件：\n————————\n\t{string.Join("\n\t", UnityTaskArgument.LogFiles.Select(file => file.RelativePath))}\n————————");
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
                GUICreateTask();
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
                GUICreateTask();
            }
        }

        /// <summary>
        /// 通过GUI创建任务
        /// </summary>
        private static void GUICreateTask()
        {
            try
            {
                UnityTaskArgument = GUITaskArgumentFactory.Intance.CreateTaskArgument<object>();
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
        private static void ParseClientLog()
        {
            if (UnityTaskArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Client) > 0)
            {
                UnityTracer.WriteLine("开始解析 [客户端] 日志文件...");

                ILogParser clientLogParser = new ClientLogParser(UnityTracer);
                clientLogParser.Parse(UnityTaskArgument);

                UnityTracer.WriteLine(
                    "[客户端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    UnityTaskArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Client && file.MonitorResults.Count > 0),
                    UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Client)),
                    UnityTaskArgument.MonitorResults.Count(result => result.LogFile.LogFileType == LogFileTypes.Client));
            }
        }

        /// <summary>
        /// 解析服务端日志
        /// </summary>
        private static void ParseServerLog()
        {
            if (UnityTaskArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Server) > 0)
            {
                UnityTracer.WriteLine("开始解析 [服务端] 日志文件...\n————————");

                ILogParser serverLogParser = new ServerLogParser(UnityTracer);
                serverLogParser.Parse(UnityTaskArgument);

                UnityTracer.WriteLine(
                    "[服务端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    UnityTaskArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Server && file.MonitorResults.Count > 0),
                    UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Server)),
                    UnityTaskArgument.MonitorResults.Count(result => result.LogFile.LogFileType == LogFileTypes.Server));
            }
        }

        /// <summary>
        /// 解析中间件日志
        /// </summary>
        private static void ParseMiddlewareLog()
        {
            if (UnityTaskArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Middleware) > 0)
            {
                UnityTracer.WriteLine("开始解析 [中间件] 日志文件...\n————————");

                ILogParser middlewareLogParser = new MiddlewareLogParser(UnityTracer);
                middlewareLogParser.Parse(UnityTaskArgument);

                UnityTracer.WriteLine(
                    "[中间件] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个结果\n————————",
                    UnityTaskArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Middleware && file.MiddlewareResults.Count > 0),
                    UnityTaskArgument.MiddlewareResults.Count);
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
            UnityTaskArgument.AnalysisResults.Clear();
            UnityTaskArgument.LogFiles.ForEach(logFile => logFile.AnalysisResults.Clear());
            UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().ToList().ForEach(monitor => monitor.AnalysisResults.Clear());

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
            return $@"{ConfigHelper.ReportExportDirectory}\任务数据转储-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xml";
        }

        /// <summary>
        /// 尝试导出日志报告
        /// </summary>
        private static void TryToExportLogReport()
        {
            string reportPath = GetReportFilePath(UnityTaskArgument);
            bool exportSuccess = false;

            // 当导出失败且用户同意重试时重复导出，并在失败时再次询问用户
            do
            {
                UnityTracer.WriteLine("开始导出日志报告...");
                try
                {
                    ExportLogReport(reportPath);
                    exportSuccess = true;
                }
                catch (Exception ex)
                {
                    exportSuccess = false;
                    UnityTracer.WriteLine($"导出日志报告失败：{ex.Message}");
                    UnityTracer.WriteLine("是否重试？(请输入： Y / N )");
                }
            }
            while (!exportSuccess && Console.ReadLine().Trim().ToUpper() == "Y");

            if (exportSuccess)
            {
                // 记录日志报告导出路径
                UnityTaskArgument.LastReportPath = reportPath;
                UnityTracer.WriteLine($"日志报告导出成功=> {UnityTaskArgument.LastReportPath}");

                // 自动打开报告
                if (File.Exists(reportPath))
                {
                    Process.Start(reportPath);
                }
            }
            else
            {
                UnityTracer.WriteLine($"放弃导出日志报告，任务结束~");
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
            return $@"{ConfigHelper.ReportExportDirectory}\xQuant导出报告-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.{argument.ReportMode.GetAmbientValue()}";
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
            Console.WriteLine("按任意键退出此程序... (￣▽￣)／");
            Console.Read();
            Environment.Exit(code);
        }

        /// <summary>
        /// 处理当前域未捕捉异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!(e.ExceptionObject is Exception UnhandledException))
            {
                return;
            }

            if (UnhandledException is OutOfMemoryException)
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
                UnhandledException.GetType().ToString(),
                UnhandledException.Source,
                UnhandledException.TargetSite.Name,
                UnhandledException.TargetSite.Module.FullyQualifiedName,
                UnhandledException.Message,
                UnhandledException.StackTrace,
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
        /// 显示解析结果
        /// </summary>
        private static void ShowParseResult()
        {
            UnityTracer.WriteLine(
                "所有日志文件解析完成：\n\t[共计] 在 {0} 个文件中发现 {1} 个监视规则的 {2} 个监视结果和 {3} 个中间件结果\n————————",
                UnityTaskArgument.LogFiles.Count(file => file.MonitorResults.Count > 0 || file.MiddlewareResults.Count > 0),
                UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Count(monitor => monitor.MonitorResults.Count > 0),
                UnityTaskArgument.MonitorResults.Count,
                UnityTaskArgument.MiddlewareResults.Count());
        }

        /// <summary>
        /// 显示分析结果
        /// </summary>
        private static void ShowAnalysisResult()
        {
            UnityTracer.WriteLine(
                "日志解析结果分析完成：\n\t在 {0} 个文件中匹配到 {1} 个监视规则的 {2} 组分析结果\n————————",
                UnityTaskArgument.LogFiles.Count(file => file.AnalysisResults.Count > 0),
                UnityTaskArgument.MonitorContainerRoot.GetMonitorItems().Count(monitor => monitor.AnalysisResults.Count > 0),
                UnityTaskArgument.AnalysisResults.Count);
            /*
            UnityTrace.WriteLine("分析结果树：");
            UnityTaskArgument.AnalysisResultContainerRoot
                .GetAnalysisResults().ToList()
                .ForEach(result => Console.WriteLine(result));
             */
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
