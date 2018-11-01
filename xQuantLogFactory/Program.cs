using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xQuantLogFactory.BIZ.FileFinder;
using xQuantLogFactory.BIZ.Parser;
using xQuantLogFactory.DAL;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory
{
    //TODO: [全局任务] 使用
    //TODO: [全局任务] 移除和排除 using
    //TODO: [全局任务] 编写单元测试

    class Program
    {
        /// <summary>
        /// 全局任务参数
        /// </summary>
        public static TaskArgument UnityArgument = null;

        /// <summary>
        /// 全局配置助手
        /// </summary>
        public static ConfigHelper UnityConfig = new ConfigHelper();

        /// <summary>
        /// 全局数据库交互对象
        /// </summary>
        public static LogDBContext UnityContext = LogDBContext.UnityContext;

        /// <summary>
        /// 全局追踪器
        /// </summary>
        public static ITrace UnityTrace = new Trace();

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

            //创建任务参数对象
            UnityTrace.WriteLine("开始创建任务参数对象...");
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

            //准备监视规则存储目录
            UnityTrace.WriteLine("准备监视规则XML文件存储目录：{0}", UnityConfig.MonitorDirectory);
            try
            {
                IOUtils.PrepareDirectory(UnityConfig.MonitorDirectory);
            }
            catch (Exception ex)
            {
                UnityTrace.WriteLine($"准备目录失败：{ex.Message}");
                Exit(2);
            }
            UnityTrace.WriteLine("准备目录成功");

            //反序列化监视规则文件
            UnityTrace.WriteLine("开始反序列化匹配的监视规则对象...");
            ITaskFileFinder monitorFinder = new MonitorFileFinder();
            UnityArgument.MonitorItems.AddRange(monitorFinder.GetFiles<MonitorItem>(UnityConfig.MonitorDirectory, UnityArgument));
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

            //获取时间段内日志文件
            UnityTrace.WriteLine("开始获取时间段内日志文件...");
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

            UnityTrace.WriteLine("开始解析日志文件...");
            ILogParser logParser = new LogParser() { UnityTrace = UnityTrace };
            foreach (MonitorResult result in logParser.Parse(UnityArgument))
            {
                /*
                result.MonitorItem;
                result.LogFile;
                 */
            }

            UnityTrace.WriteLine("日志文件解析完成");

            //TODO: so much todo ...

            //记录任务完成时间
            UnityArgument.TaskFinishTime = DateTime.Now;
            UnityContext.SaveChanges();

            Exit(0);
        }

        /// <summary>
        /// 程序结束
        /// </summary>
        /// <param name="code">程序退出代码 (0: 正常退出)</param>
        public static void Exit(int code)
        {
            UnityContext?.Dispose();

            Console.WriteLine("\n————————\n按任意键退出此程序... (￣▽￣)／");
            Console.Read();
            Environment.Exit(code);
        }

    }
}
