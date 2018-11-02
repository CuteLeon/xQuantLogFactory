using System;
using System.Linq;
using System.Windows.Forms;

using xQuantLogFactory.BIZ.FileFinder;
using xQuantLogFactory.BIZ.Parser;
using xQuantLogFactory.DAL;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory
{
    //TODO: 日志文件通过并行解析，数据库内记录在日志文件范围内以日至时间和日志行号有序，但对整个任务是无序的，可以通过日志时间大致排序(有概率重复)或区分文件以日志行号排序；

    //TODO: [全局任务] 使用4.0或5.0语言版本...
    //TODO: [全局任务] 移除和排除 using
    //TODO: [全局任务] 编写单元测试

    class Program
    {
        /// <summary>
        /// 全局任务参数
        /// </summary>
        public volatile static TaskArgument UnityArgument = null;

        /// <summary>
        /// 全局数据库交互对象
        /// </summary>
        public static LogDBContext UnityContext = LogDBContext.UnityContext;

        /// <summary>
        /// 全局追踪器
        /// </summary>
        public volatile static ITrace UnityTrace = new Trace();

        /* 启动参数：{string_日志文件目录} {string.Format(,)_监控的项目名称列表} "{datetime_日志开始时间}" "[datetime_日志截止时间 =DateTime.Now]" [boolean_包含系统信息 =false] [boolean_包含客户端信息 =false] [reportmodes_报告导出模式 =RepostModes.Html]
         * 注意：
         *  1. 任何参数值内含有空格时需要在值外嵌套英文双引号；
         *  2. 不允许离散地省略参数，只可以选择省略某可选参数及其后所有的参数
         * 参数介绍：
         * {string_日志文件目录}：目录含有空格时需要在值外嵌套英文双引号；如：C:\TEST_DIR 或 "C:\TEST DIR" 
         * {string.Format(,)_监控的项目名称列表}：程序监控的项目名称列表；不允许为空；当存在多个值时，值间以英文逗号分隔(不加空格)；含有空格时需要在值外嵌套英文双引号，如：监控项目_Demo 或 监控项目_0,监控项目_1 或 "监控项目_0,监控项目 1"
         * {datetime_日志开始时间}：以格式化日期时间传入；采用24小时制；格式如：yyyy-MM-dd HH:mm:ss
         * {datetime_日志截止时间 =DateTime.Now}：格式同日志开始时间；采用24小时制；可省略，默认值为当前时间
         * {boolean_包含系统信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * {boolean_包含客户端信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * {reportmodes_报告导出模式 =RepostModes.Html}：可省略，默认值为 Html；可取值为：{html/word/excel}，可忽略大小写
         */

        /// <summary>
        /// 入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = $"xQuant 日志分析工具 - {Application.ProductVersion}";
            UnityTrace.WriteLine($"{Console.Title} 已启动...");
            UnityTrace.WriteLine($"启动参数：\n————————\n\t{string.Join("\n\t", args)}\n————————");

            UnityTrace.WriteLine("开始创建任务参数对象...");
            CreateTaskArgument(args);

            UnityTrace.WriteLine("准备监视规则XML文件存储目录：{0}", ConfigHelper.MonitorDirectory);
            CheckMonitorDirectory(ConfigHelper.MonitorDirectory);

            UnityTrace.WriteLine("开始反序列化匹配的监视规则对象...");
            GetMonitorItems(ConfigHelper.MonitorDirectory);

            UnityTrace.WriteLine("开始获取时间段内日志文件...");
            GetTaskLogFiles(UnityArgument.BaseDirectory);

            UnityTrace.WriteLine("开始解析日志文件...");
            ParseClientLog();
            ParseServerLog();
            ParseMiddlewareLog();

            ShowParseResult();

            //TODO: so much todo ...

            //记录任务完成时间
            UnityArgument.TaskFinishTime = DateTime.Now;
            UnityContext.SaveChanges();

            Exit(0);
        }

        /// <summary>
        /// 获取任务相关日志文件
        /// </summary>
        /// <param name="directory"></param>
        private static void GetTaskLogFiles(string directory)
        {
            ITaskFileFinder logFinder = new LogFileFinder();
            UnityArgument.LogFiles.AddRange(logFinder.GetFiles<LogFile>(UnityArgument.BaseDirectory, UnityArgument));
            UnityContext.SaveChanges();
            if (UnityArgument.LogFiles.Count == 0)
            {
                UnityTrace.WriteLine("未发现任务相关日志文件，程序将退出");
                Exit(4);
            }
            else
            {
                UnityTrace.WriteLine($"发现 {UnityArgument.LogFiles.Count} 个日志文件：\n————————\n\t{string.Join("\n\t", UnityArgument.LogFiles.Select(file => file.FilePath))}\n————————");
            }
        }

        /// <summary>
        /// 获取监视规则对象
        /// </summary>
        /// <param name="directory"></param>
        private static void GetMonitorItems(string directory)
        {
            ITaskFileFinder monitorFinder = new MonitorFileFinder();
            UnityArgument.MonitorItems.AddRange(monitorFinder.GetFiles<MonitorItem>(directory, UnityArgument));
            UnityContext.SaveChanges();
            if (UnityArgument.MonitorItems.Count == 0)
            {
                UnityTrace.WriteLine("未发现任务相关监视规则，程序将退出");
                Exit(3);
            }
            else
            {
                UnityTrace.WriteLine($"发现 {UnityArgument.MonitorItems.Count} 个任务相关监视规则对象：{string.Join("、", UnityArgument.MonitorItems.Select(item => item.Name))}");
            }
        }

        /// <summary>
        /// 检查监视规则目录
        /// </summary>
        /// <param name="directory"></param>
        private static void CheckMonitorDirectory(string directory)
        {
            try
            {
                IOUtils.PrepareDirectory(directory);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"准备目录失败：{ex.Message}");
                Exit(2);
            }
            UnityTrace.WriteLine("准备目录成功");
        }

        /// <summary>
        /// 创建任务参数
        /// </summary>
        /// <param name="args"></param>
        private static void CreateTaskArgument(string[] args)
        {
            try
            {
                UnityArgument = TaskArgument.Parse(args);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"创建任务参数对象失败：{ex.Message}");
                Exit(1);
            }
            UnityContext.TaskArguments.Add(UnityArgument);
            UnityContext.SaveChanges();
            UnityTrace.WriteLine("创建任务参数对象成功：\n————————\n{0}\n————————", UnityArgument);
        }

        /// <summary>
        /// 程序结束
        /// </summary>
        /// <param name="code">程序退出代码 (0: 正常退出)</param>
        public static void Exit(int code)
        {
            UnityContext?.Dispose();

            Console.WriteLine("\n————————");
            ShowTaskDuration();
            Console.WriteLine("————————");
            Console.WriteLine("按任意键退出此程序... (￣▽￣)／");
            Console.Read();
            Environment.Exit(code);
        }

        /// <summary>
        /// 显示任务耗时
        /// </summary>
        private static void ShowTaskDuration()
        {
            if (UnityArgument != null)
            {
                TimeSpan duration = UnityArgument.TaskFinishTime.Subtract(UnityArgument.TaskStartTime);
                Console.WriteLine($"任务耗时：{duration.Hours}时 {duration.Minutes}分 {duration.Seconds}秒 {duration.Milliseconds}毫秒");
            }
        }

        /// <summary>
        /// 显示解析结果
        /// </summary>
        private static void ShowParseResult()
        {
            UnityTrace.WriteLine("所有日志文件解析完成：\n\t[共计] 在 {0} 个文件中发现 {1} 个监视规则的 {2} 个监视结果和 {3} 个中间件结果\n————————",
                UnityArgument.LogFiles.Count(file => file.MonitorResults.Count > 0 || file.MiddlewareResults.Count > 0),
                UnityArgument.MonitorItems.Count(monitor => monitor.MonitorResults.Count > 0),
                UnityArgument.MonitorResults.Count,
                UnityArgument.MiddlewareResults.Count()
                );
        }

        /// <summary>
        /// 解析客户端日志
        /// </summary>
        private static void ParseClientLog()
        {
            if (UnityArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Client) > 0)
            {
                UnityTrace.WriteLine("开始解析 [客户端] 日志文件...\n————————");

                LogParserBase clientLogParser = new ClientLogParser(UnityTrace);
                clientLogParser.Parse(UnityArgument);

                lock (UnityContext)
                    UnityContext.SaveChanges();

                UnityTrace.WriteLine("[客户端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    UnityArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Client && file.MonitorResults.Count > 0),
                    UnityArgument.MonitorItems.Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Client)),
                    UnityArgument.MonitorResults.Count(result => result.LogFile.LogFileType == LogFileTypes.Client)
                    );
            }
        }

        /// <summary>
        /// 解析服务端日志
        /// </summary>
        private static void ParseServerLog()
        {
            if (UnityArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Server) > 0)
            {
                UnityTrace.WriteLine("开始解析 [服务端] 日志文件...\n————————");

                LogParserBase serverLogParser = new ServerLogParser(UnityTrace);
                serverLogParser.Parse(UnityArgument);

                lock (UnityContext)
                    UnityContext.SaveChanges();

                UnityTrace.WriteLine("[服务端] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个监视规则的 {2} 个结果\n————————",
                    UnityArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Server && file.MonitorResults.Count > 0),
                    UnityArgument.MonitorItems.Count(monitor => monitor.MonitorResults.Any(result => result.LogFile.LogFileType == LogFileTypes.Server)),
                    UnityArgument.MonitorResults.Count(result => result.LogFile.LogFileType == LogFileTypes.Server)
                    );
            }
        }

        /// <summary>
        /// 解析中间件日志
        /// </summary>
        private static void ParseMiddlewareLog()
        {
            if (UnityArgument.LogFiles.Count(logFile => logFile.LogFileType == LogFileTypes.Middleware) > 0)
            {
                UnityTrace.WriteLine("开始解析 [中间件] 日志文件...\n————————");

                LogParserBase middlewareLogParser = new MiddlewareLogParser(UnityTrace);
                middlewareLogParser.Parse(UnityArgument);

                lock (UnityContext)
                    UnityContext.SaveChanges();

                UnityTrace.WriteLine("[中间件] 日志文件解析完成：\n\t在 {0} 个文件中发现 {1} 个结果\n————————",
                    UnityArgument.LogFiles.Count(file => file.LogFileType == LogFileTypes.Middleware && file.MiddlewareResults.Count > 0),
                    UnityArgument.MiddlewareResults.Count
                    );
            }
        }

    }
}
