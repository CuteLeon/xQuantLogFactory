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
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Utils;
using xQuantLogFactory.Utils.Extensions;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory
{

    //日志文件通过并行解析，数据库内记录在日志文件范围内以日志时间和日志行号有序，但对整个任务是无序的，可以通过日志时间大致排序(有概率重复)或区分文件以日志行号排序；

    //TODO: 外汇2.0、窗口加载等需求通过实现独立的分析器完成，不改变解析器逻辑，以免降低性能

    //TODO: 序列化 UnityTaskArgument 为xml
    //TODO: 不约束参数顺序，
    //TODO: 命令行参数增加一个日志级别，可以只分析指定日志级别的日志文件，默认为 DEBUG，多个时按"|"分割直接插入Config日志文件名

    //TODO: [全局任务] 使用4.0或5.0语言版本...
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

        /* 启动参数：{string_日志文件目录} {string.Format(,)_监控的项目名称列表} "{datetime_日志开始时间}" "[datetime_日志截止时间 =DateTime.Now]" [boolean_包含系统信息 =false] [boolean_包含客户端信息 =false] [reportmodes_报告导出模式 =RepostModes.Html]
         * 注意：
         *  1. 任何参数值内含有空格时需要在值外嵌套英文双引号；
         *  2. 不允许离散地省略参数，只可以选择省略某可选参数及其后所有的参数
         * 参数介绍：
         * {string_日志文件目录}：目录含有空格时需要在值外嵌套英文双引号；如：C:\TEST_DIR 或 "C:\TEST DIR" 
         * {string.Format(,)_监控的项目名称列表}：可省略，默认为所有监视规则；程序监控的项目名称列表；当存在多个值时，值间以英文逗号分隔(不加空格)；含有空格时需要在值外嵌套英文双引号，如：监控项目_Demo 或 监控项目_0,监控项目_1 或 "监控项目_0,监控项目 1"
         * {datetime_日志开始时间}：可省略，以格式化日期时间传入；采用24小时制；格式如：yyyy-MM-dd HH:mm:ss
         * {datetime_日志截止时间 =DateTime.Now}：可省略，默认值为当前时间；格式同日志开始时间；采用24小时制；
         * {boolean_包含系统信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * {boolean_包含客户端信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * {reportmodes_报告导出模式 =RepostModes.Html}：可省略，默认值为 Html；可取值为：{html/word/excel}，可忽略大小写
         */

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
            UnityDBContext.Database.Log = SQLTrace.WriteLine;
#endif

#if (!DEBUG)
            UnityTaskArgument = UnityDBContext.TaskArguments.OrderByDescending(task => task.TaskStartTime).FirstOrDefault();
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

            UnityTrace.WriteLine("开始反序列化匹配的监视规则对象...");
            GetMonitorItems(ConfigHelper.MonitorDirectory);
            UnityTrace.WriteLine($"发现 {UnityTaskArgument.MonitorItems.Count} 个任务相关监视规则对象：{string.Join("、", UnityTaskArgument.MonitorItems.Select(item => item.Name))}");

            UnityTrace.WriteLine("开始获取时间段内日志文件...");
            GetTaskLogFiles(UnityTaskArgument.LogDirectory);

            UnityTrace.WriteLine("开始解析日志文件...");
            //未发现监视规则对象，不解析客户端和服务端日志文件
            if (UnityTaskArgument.MonitorItems.Count > 0)
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

            //SaveTaskArgumentToXML();
            TryToExportLogReport();

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
                Exit(4);
            }
            else
            {
                UnityTrace.WriteLine($"发现 {UnityTaskArgument.LogFiles.Count} 个日志文件：\n————————\n\t{string.Join("\n\t", UnityTaskArgument.LogFiles.Select(file => file.FilePath))}\n————————");
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
                Exit(5);
            }

            if (monitorItems.Count() > 0)
            {
                UnityTaskArgument.MonitorItemTree.AddRange(monitorItems);
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
            try
            {
                UnityTaskArgument = TaskArgument.Parse(args);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"创建任务参数对象失败：{ex.Message}");
                Exit(1);
            }
            UnityDBContext.TaskArguments.Add(UnityTaskArgument);
            UnityDBContext.SaveChanges();
            UnityTrace.WriteLine("创建任务参数对象成功：\n————————\n{0}\n————————", UnityTaskArgument);
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
                    UnityTaskArgument.MonitorItems.Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Client)),
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
                    UnityTaskArgument.MonitorItems.Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Server)),
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
            UnityTaskArgument.MonitorItems.ForEach(monitor => monitor.AnalysisResults.Clear());
            lock (UnityDBContext)
                UnityDBContext.SaveChanges();

            ILogAnalysiser logAnalysiser = new GroupLogAnalysiser(UnityTrace);
            logAnalysiser.Analysis(UnityTaskArgument);

            lock (UnityDBContext)
                UnityDBContext.SaveChanges();
        }

        #endregion

        #region 导出报告

        /// <summary>
        /// 将任务储存为XML文件
        /// </summary>
        private static void SaveTaskArgumentToXML()
        {
            try
            {
                string taskXMLPath = GetXMLFilePath(UnityTaskArgument);
                string xmlContent = UnityTaskArgument.SerializeToXML();
                File.WriteAllText(taskXMLPath, xmlContent);
                UnityTrace.WriteLine($"任务对象存储到XML成功：{taskXMLPath}");
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
            return $@"{ConfigHelper.ReportExportDirectory}\{argument.TaskID}.xml";
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
                        reportExporter = new ExcelLogReportExporter();
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
            return $@"{ConfigHelper.ReportExportDirectory}\{argument.TaskID}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.{argument.ReportMode.GetAmbientValue()}";
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
                "应用域内发现未被捕获的异常：\r\n" +
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
                UnityTaskArgument.MonitorItems.Count(monitor => monitor.MonitorResults.Count > 0),
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
                UnityTaskArgument.MonitorItems.Count(monitor => monitor.AnalysisResults.Count > 0),
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
