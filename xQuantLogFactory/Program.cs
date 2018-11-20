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
using xQuantLogFactory.DAL;
using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Factory;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Extensions;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory
{

    //TODO: 服务端+客户端监视规则配置

    //TODO: [全局任务] 移除和排除 using
    //TODO: [全局任务] 编写单元测试

    class Program
    {
        /// <summary>
        /// 全局任务参数
        /// </summary>
        public volatile static TaskArgument UnityTaskArgument = null;

        /// <summary>
        /// 全局数据库交互对象
        /// </summary>
        public static LogDBContext UnityDBContext = LogDBContext.UnityDBContext;

        /// <summary>
        /// SQL追踪器
        /// </summary>
        public static ITracer SQLTrace = new SQLTracer();

        /// <summary>
        /// 全局追踪器
        /// </summary>
        public volatile static ITracer UnityTrace = new ConsoleTracer();

        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="args">启动参数</param>
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.Title = $"xQuant 日志分析工具 - {Application.ProductVersion}";
            UnityTrace.WriteLine($"{Console.Title} 已启动...");
            UnityTrace.WriteLine($"启动参数：\n————————\n\t{string.Join("\n\t", args)}\n————————");

#if (DEBUG)
            //输出SQL日志
            //UnityDBContext.Database.Log = SQLTrace.WriteLine;
#endif

#if (!DEBUG)
            UnityTaskArgument = UnityDBContext.TaskArguments.OrderByDescending(task => task.TaskStartTime).FirstOrDefault();
            if (UnityTaskArgument == null)
            {
                Console.WriteLine("未在数据库返回任务对象，请切换 [DEBUG] 状态完成一次任务！");
                Exit(int.MaxValue - 1);
            }
            //手动加载导航数据
            if (!UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.AnalysisResults).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.AnalysisResults).Load();
            if (!UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.LogFiles).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.LogFiles).Load();
            if (!UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.MiddlewareResults).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.MiddlewareResults).Load();
            if (!UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.MonitorResults).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument).Collection(argument => argument.MonitorResults).Load();
            if (!UnityDBContext.Entry(UnityTaskArgument).Reference(argument => argument.SystemInfo).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument).Reference(argument => argument.SystemInfo).Load();
            if (!UnityDBContext.Entry(UnityTaskArgument).Reference(argument => argument.MonitorRoot).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument).Reference(argument => argument.MonitorRoot).Load();
            if (!UnityDBContext.Entry(UnityTaskArgument.MonitorRoot).Collection(monitorRoot => monitorRoot.MonitorTreeRoots).IsLoaded)
                UnityDBContext.Entry(UnityTaskArgument.MonitorRoot).Collection(monitorRoot => monitorRoot.MonitorTreeRoots).Load();

            UnityTaskArgument.TaskStartTime = DateTime.Now;
            UnityTrace.WriteLine("当前任务参数信息：\n————————\n{0}\n————————", UnityTaskArgument);
#else
            UnityTrace.WriteLine("开始创建任务参数对象...");
            CreateTaskArgument(args);

            if (UnityTaskArgument?.IncludeSystemInfo ?? false)
            {
                UnityTrace.WriteLine("开始获取系统信息...");
                GetSystemInfo();
            }

            UnityTrace.WriteLine("准备监视规则XML文件存储目录：{0}", ConfigHelper.MonitorDirectory);
            CheckDirectory(ConfigHelper.MonitorDirectory);

            UnityTrace.WriteLine("准备日志报告导出目录：{0}", ConfigHelper.ReportExportDirectory);
            CheckDirectory(ConfigHelper.ReportExportDirectory);

            UnityTrace.WriteLine("准备报告导出模板目录：{0}", ConfigHelper.ReportTempletDirectory);
            CheckDirectory(ConfigHelper.ReportTempletDirectory);

            UnityTrace.WriteLine("开始反序列化匹配的监视规则对象...");
            GetMonitorItems(ConfigHelper.MonitorDirectory);
            UnityTrace.WriteLine($"发现 {UnityTaskArgument.MonitorRoot.MonitorItems.Count} 个任务相关监视规则对象：{string.Join("、", UnityTaskArgument.MonitorRoot.MonitorItems.Select(item => item.Name))}");

            UnityTrace.WriteLine("开始获取任务相关日志文件...");
            GetTaskLogFiles(UnityTaskArgument.LogDirectory);

            UnityTrace.WriteLine("开始解析日志文件...");
            //未发现监视规则对象，不解析客户端和服务端日志文件
            if (UnityTaskArgument.MonitorRoot.MonitorItems.Count > 0)
            {
                ParseClientLog();
                ParseServerLog();
            }
            ParseMiddlewareLog();
            ShowParseResult();

            UnityTrace.WriteLine("开始分析日志解析结果...");
            AnalysisLog();
#endif
            ShowAnalysisResult();

            UnityTaskArgument.TaskFinishTime = DateTime.Now;
            UnityDBContext.SaveChanges();

            TryToExportLogReport();
            SaveTaskArgumentToXML(UnityTaskArgument.DeepClone());

            Exit(0);
        }

        #region 准备任务及相关数据

        /// <summary>
        /// 获取任务相关日志文件
        /// </summary>
        /// <param name="directory"></param>
        private static void GetTaskLogFiles(string directory)
        {
            ITaskFileFinder logFinder = new LogFileFinder();
            IEnumerable<LogFile> logFiles = null;

            try
            {
                if (!Directory.Exists(directory)) throw new DirectoryNotFoundException($"不存在的日志文件目录：{directory}");

                logFiles = logFinder.GetFiles<LogFile>(UnityTaskArgument.LogDirectory, UnityTaskArgument);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"获取任务相关日志文件失败：{ex.Message}");
                Exit(4);
            }

            if (logFiles.Count() > 0)
            {
                UnityTaskArgument.LogFiles.AddRange(logFiles);
                UnityDBContext.SaveChanges();
            }

            if (UnityTaskArgument.LogFiles.Count == 0)
            {
                UnityTrace.WriteLine("未发现任务相关日志文件，程序将退出");
                Exit(3);
            }
            else
            {
                UnityTrace.WriteLine($"发现 {UnityTaskArgument.LogFiles.Count} 个日志文件：\n————————\n\t{string.Join("\n\t", UnityTaskArgument.LogFiles.Select(file => file.RelativePath))}\n————————");
            }
        }

        /// <summary>
        /// 获取监视规则对象
        /// </summary>
        /// <param name="directory"></param>
        private static void GetMonitorItems(string directory)
        {
            ITaskFileFinder monitorFinder = new MonitorFileFinder();
            IEnumerable<MonitorItem> monitorItems = null;

            try
            {
                monitorItems = monitorFinder.GetFiles<MonitorItem>(directory, UnityTaskArgument);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"获取任务相关监视规则失败：{ex.Message}");
                Exit(4);
            }

            if (monitorItems.Count() > 0)
            {
                //TODO: 优化去掉
                if (UnityTaskArgument.MonitorRoot == null) UnityTaskArgument.MonitorRoot = new MonitorContainer();
                UnityTaskArgument.MonitorRoot.MonitorTreeRoots.AddRange(monitorItems);
                UnityDBContext.SaveChanges();
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
                UnityTrace.WriteLine($"准备工作目录失败：{ex.Message}");
                Exit(2);
            }
            UnityTrace.WriteLine("准备工作目录成功");
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
                throw new ArgumentException("创建任务失败！");

            UnityDBContext.TaskArguments.Add(UnityTaskArgument);
            UnityDBContext.SaveChanges();
            UnityTrace.WriteLine("创建任务参数对象成功：\n————————\n{0}\n————————", UnityTaskArgument);
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
                UnityTrace.WriteLine($"创建任务参数对象失败：{ex.Message}");
                UnityTrace.WriteLine(ArgsTaskArgumentFactory.Intance.Usage);

                UnityTrace.WriteLine($"正在使用 GUI 创建任务：");
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
                UnityTrace.WriteLine($"创建任务参数对象失败：{ex.Message}");
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
            UnityDBContext.SaveChanges();
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
                UnityTrace.WriteLine("开始解析 [客户端] 日志文件...\n————————");

                ILogParser clientLogParser = new ClientLogParser(UnityTrace);
                clientLogParser.Parse(UnityTaskArgument);

                lock (UnityDBContext)
                    UnityDBContext.SaveChanges();

                UnityTrace.WriteLine("[客户端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    UnityTaskArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Client && file.MonitorResults.Count > 0),
                    UnityTaskArgument.MonitorRoot.MonitorItems.Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Client)),
                    UnityTaskArgument.MonitorResults.Count(result => result.LogFile.LogFileType == LogFileTypes.Client)
                    );
            }
        }

        /// <summary>
        /// 解析服务端日志
        /// </summary>
        private static void ParseServerLog()
        {
            if (UnityTaskArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Server) > 0)
            {
                UnityTrace.WriteLine("开始解析 [服务端] 日志文件...\n————————");

                ILogParser serverLogParser = new ServerLogParser(UnityTrace);
                serverLogParser.Parse(UnityTaskArgument);

                lock (UnityDBContext)
                    UnityDBContext.SaveChanges();

                UnityTrace.WriteLine("[服务端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    UnityTaskArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Server && file.MonitorResults.Count > 0),
                    UnityTaskArgument.MonitorRoot.MonitorItems.Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Server)),
                    UnityTaskArgument.MonitorResults.Count(result => result.LogFile.LogFileType == LogFileTypes.Server)
                    );
            }
        }

        /// <summary>
        /// 解析中间件日志
        /// </summary>
        private static void ParseMiddlewareLog()
        {
            if (UnityTaskArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Middleware) > 0)
            {
                UnityTrace.WriteLine("开始解析 [中间件] 日志文件...\n————————");

                ILogParser middlewareLogParser = new MiddlewareLogParser(UnityTrace);
                middlewareLogParser.Parse(UnityTaskArgument);

                lock (UnityDBContext)
                    UnityDBContext.SaveChanges();

                UnityTrace.WriteLine("[中间件] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个结果\n————————",
                    UnityTaskArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Middleware && file.MiddlewareResults.Count > 0),
                    UnityTaskArgument.MiddlewareResults.Count
                    );
            }
        }

        #endregion

        #region 分析日志

        /// <summary>
        /// 分析日志解析结果
        /// </summary>
        private static void AnalysisLog()
        {
            //分析结果前先清空分析结果
            UnityDBContext.AnalysisResults.RemoveRange(UnityTaskArgument.AnalysisResults);
            UnityTaskArgument.AnalysisResults.Clear();
            UnityTaskArgument.LogFiles.ForEach(logFile => logFile.AnalysisResults.Clear());
            UnityTaskArgument.MonitorRoot.MonitorItems.ForEach(monitor => monitor.AnalysisResults.Clear());
            lock (UnityDBContext)
                UnityDBContext.SaveChanges();

            LogAnalysiserHost logAnalysiserHost = new GroupLogAnalysiser(UnityTrace);
            DirectedLogAnalysiserBase formLogAnalysiser = new CommonPrefixAnalysiser(UnityTrace) { TargetMonitorName = "打开功能模块" };
            DirectedLogAnalysiserBase reportLogAnalysiser = new CommonPrefixAnalysiser(UnityTrace) { TargetMonitorName = "查询报表" };

            logAnalysiserHost.AddAnalysiser(formLogAnalysiser);
            logAnalysiserHost.AddAnalysiser(reportLogAnalysiser);

            logAnalysiserHost.Analysis(UnityTaskArgument);

            lock (UnityDBContext)
                UnityDBContext.SaveChanges();
        }

        #endregion

        #region 导出报告

        /// <summary>
        /// 将任务储存为XML文件
        /// </summary>
        /// <param name="argument">任务参数对象</param>
        private static void SaveTaskArgumentToXML(TaskArgument argument)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));

            try
            {
                string taskXMLPath = GetXMLFilePath(argument);
                string xmlContent = argument.SerializeToXML();
                File.WriteAllText(taskXMLPath, xmlContent);
                UnityTrace.WriteLine($"任务对象存储到XML成功：{taskXMLPath}");

                //Process.Start("notepad.exe", taskXMLPath);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"任务对象存储到XML失败：{ex.Message}");
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
            //当导出失败且用户同意重试时重复导出，并在失败时再次询问用户
            do
            {
                UnityTrace.WriteLine("开始导出日志报告...");
                try
                {
                    ExportLogReport(reportPath);
                    exportSuccess = true;
                }
                catch (Exception ex)
                {
                    exportSuccess = false;
                    UnityTrace.WriteLine($"导出日志报告失败：{ex.Message}");
                    UnityTrace.WriteLine("是否重试？(请输入： Y / N )");
                }
            } while (!exportSuccess && Console.ReadLine().Trim().ToUpper() == "Y");

            if (exportSuccess)
            {
                //记录日志报告导出路径
                UnityTaskArgument.LastReportPath = reportPath;
                UnityTrace.WriteLine($"日志报告到处成功=> {UnityTaskArgument.LastReportPath}");

                //自动打开报告
                if (File.Exists(reportPath)) Process.Start(reportPath);
            }
            else
            {
                UnityTrace.WriteLine($"放弃导出日志报告，任务结束~");
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
                        reportExporter = new ExcelLogReportExporter(UnityTrace);
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
        public static void Exit(int code)
        {
            //记录任务完成时间
            if (UnityTaskArgument != null &&
                (UnityTaskArgument.TaskFinishTime == DateTime.MinValue ||
                 UnityTaskArgument.TaskFinishTime == null)
                )
            {
                UnityTaskArgument.TaskFinishTime = DateTime.Now;
                UnityDBContext.SaveChanges();
            }
            UnityDBContext.Dispose();

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
            if (!(e.ExceptionObject is Exception UnhandledException)) return;

            if (UnhandledException is OutOfMemoryException)
            {
                GC.KeepAlive(UnityTaskArgument);
                GC.Collect();

                if (!e.IsTerminating) return;
            }

            string ExceptionDescription = string.Format(
                "应用域内发现未被捕获的异常：(;′⌒`)\r\n" +
                "\t异常类型 : {0}\r\n" +
                "\t异常地址 : {1}\r\n" +
                "\t出错方法 : {2}\r\n" +
                "\t所在文件 : {3}\r\n" +
                "\t异常信息 : {4}\r\n" +
                "\t调用堆栈 : \r\n{5}\r\n" +
                "\t即将终止 : {6}",
                UnhandledException.GetType().ToString(),
                UnhandledException.Source,
                UnhandledException.TargetSite.Name,
                UnhandledException.TargetSite.Module.FullyQualifiedName,
                UnhandledException.Message,
                UnhandledException.StackTrace,
                e.IsTerminating
            );

            Console.WriteLine("——————————————————");
            UnityTrace.WriteLine(ExceptionDescription);
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
            UnityTrace.WriteLine("所有日志文件解析完成：\n\t[共计] 在 {0} 个文件中发现 {1} 个监视规则的 {2} 个监视结果和 {3} 个中间件结果\n————————",
                UnityTaskArgument.LogFiles.Count(file => file.MonitorResults.Count > 0 || file.MiddlewareResults.Count > 0),
                UnityTaskArgument.MonitorRoot.MonitorItems.Count(monitor => monitor.MonitorResults.Count > 0),
                UnityTaskArgument.MonitorResults.Count,
                UnityTaskArgument.MiddlewareResults.Count()
                );
        }

        /// <summary>
        /// 显示分析结果
        /// </summary>
        private static void ShowAnalysisResult()
        {
            UnityTrace.WriteLine("日志解析结果分析完成：\n\t在 {0} 个文件中匹配到 {1} 个监视规则的 {2} 组分析结果\n————————",
                UnityTaskArgument.LogFiles.Count(file => file.AnalysisResults.Count > 0),
                UnityTaskArgument.MonitorRoot.MonitorItems.Count(monitor => monitor.AnalysisResults.Count > 0),
                UnityTaskArgument.AnalysisResults.Count
                );
        }

        /// <summary>
        /// 输出系统信息
        /// </summary>
        private static void ShowSystemInfo()
        {
            UnityTrace.WriteLine($"获取系统信息完成：\n————————\n{UnityTaskArgument.SystemInfo?.ToString()}\n————————");
        }

        #endregion

    }
}
