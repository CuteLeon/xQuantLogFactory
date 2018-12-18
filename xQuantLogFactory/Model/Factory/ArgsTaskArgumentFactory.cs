using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model.Factory
{
    /// <summary>
    /// 命令行-任务参数对象工厂
    /// </summary>
    public class ArgsTaskArgumentFactory : ITaskArgumentFactory
    {
        /// <summary>
        /// 日志文件目录
        /// </summary>
        public const string LOG_DIR = "logdir";

        /// <summary>
        /// 监视规则文件名称
        /// </summary>
        public const string MONITOR_NAME = "monitor";

        /// <summary>
        /// 日志开始时间
        /// </summary>
        public const string START_TIME = "start";

        /// <summary>
        /// 日志截止时间
        /// </summary>
        public const string FINISH_TIME = "finish";

        /// <summary>
        /// 包含系统信息
        /// </summary>
        public const string SYS_INFO = "sysinfo";

        /// <summary>
        /// 包含客户端信息
        /// </summary>
        public const string CLIENT_INFO = "cltinfo";

        /// <summary>
        /// 报告导出模式
        /// </summary>
        public const string REPORT_MODE = "report";

        /// <summary>
        /// 日志等级
        /// </summary>
        public const string LOG_LEVEL = "level";

        /// <summary>
        /// 自动退出
        /// </summary>
        public const string AUTO_EXIT = "exit";

        /// <summary>
        /// 自动打开报告
        /// </summary>
        public const string AUTO_OPEN_REPORT = "open";

        /// <summary>
        /// 参数匹配正则表达式
        /// </summary>
        private static readonly Regex ArgRegex = new Regex(@"^(?<ArgName>.*)=(?<ArgValue>.*?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Lazy<StringBuilder> UsageBuilder = new Lazy<StringBuilder>();
        private static Lazy<ArgsTaskArgumentFactory> factory = new Lazy<ArgsTaskArgumentFactory>();
        private static Lazy<Dictionary<string, (string, string)>> argumentDescription = new Lazy<Dictionary<string, (string, string)>>();

        /// <summary>
        /// Gets 任务参数工厂实例
        /// </summary>
        public static ArgsTaskArgumentFactory Intance
        {
            get => factory.Value;
        }

        /// <summary>
        /// Gets 参数使用方法
        /// </summary>
        public string Usage
        {
            get
            {
                if (!UsageBuilder.IsValueCreated)
                {
                    UsageBuilder.Value.AppendLine($"{Console.Title} 命令参数说明：");
                    UsageBuilder.Value.AppendLine();

                    UsageBuilder.Value.AppendLine("参数格式规范：");
                    UsageBuilder.Value.AppendLine("\t1.参数传入格式：[参数名称]=[参数数据]");
                    UsageBuilder.Value.AppendLine("\t2.参数名称和数据内存在空格时，需要在外面嵌套英文双引号");
                    UsageBuilder.Value.AppendLine("\t3.参数名称可忽略大小写");
                    UsageBuilder.Value.AppendLine();

                    UsageBuilder.Value.AppendLine("参数说明：");
                    UsageBuilder.Value.AppendLine("\t<名称>\t\t<要求>\t\t<描述>");
                    foreach (var arg in ArgumentDescriptions)
                    {
                        UsageBuilder.Value.AppendLine($"\t{arg.Key}\t\t{arg.Value.Item1}\t\t{arg.Value.Item2}");
                    }

                    UsageBuilder.Value.AppendLine();

                    UsageBuilder.Value.AppendLine("参数示例：");
                    UsageBuilder.Value.AppendLine("\tlogdir=D:\\Desktop\\LogDir \"finish=2018-11-11 18:30:00\" monitor=client.xml report=excel level=debug");
                }

                return UsageBuilder.Value.ToString();
            }
        }

        /// <summary>
        /// Gets 参数描述
        /// </summary>
        private static Dictionary<string, (string, string)> ArgumentDescriptions
        {
            get
            {
                if (!argumentDescription.IsValueCreated)
                {
                    argumentDescription.Value.Add(LOG_DIR, ("*必选", "日志文件存放目录，如：D:\\Log 或 \"E:\\Log Dir\""));
                    argumentDescription.Value.Add(MONITOR_NAME, ("*必选", "监视规则文件名称，如：monitor.xml"));
                    argumentDescription.Value.Add(START_TIME, (" 可选", "日志开始时间，如：\"2018-10-01 17:30:00\""));
                    argumentDescription.Value.Add(FINISH_TIME, (" 可选", "日志结束时间，如：\"2018-11-11 08:40:00\""));
                    argumentDescription.Value.Add(SYS_INFO, (" 可选", "是否记录系统信息，如：true 或 false"));
                    argumentDescription.Value.Add(CLIENT_INFO, (" 可选", "是否记录客户端信息，如：true 或 false"));
                    argumentDescription.Value.Add(REPORT_MODE, (" 可选", "导出报告格式，如：excel 或 html 或 word"));
                    argumentDescription.Value.Add(LOG_LEVEL, (" 可选", "日志等级，如：debug 或 trace 或 info 或 pref 等"));
                    argumentDescription.Value.Add(AUTO_EXIT, (" 可选", "自动退出工具，如：true 或 false"));
                    argumentDescription.Value.Add(AUTO_OPEN_REPORT, (" 可选", "自动打开报告，如：true 或 false"));
                }

                return argumentDescription.Value;
            }
        }

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">工具启动参数</param>
        /// <returns>任务参数对象</returns>
        public TaskArgument CreateTaskArgument<T>(T source)
            where T : class
        {
            if (!(source is string[] args) || args.Length == 0)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Dictionary<string, string> argumentDict = new Dictionary<string, string>();

            // 解析参数并录入字典
            Match argMatch = null;
            foreach (var arg in args)
            {
                argMatch = ArgRegex.Match(arg.Trim('\"'));
                if (argMatch.Success &&
                    argMatch.Groups["ArgName"].Success &&
                    argMatch.Groups["ArgValue"].Success)
                {
                    argumentDict[argMatch.Groups["ArgName"].Value] = argMatch.Groups["ArgValue"].Value;
                }
            }

            // 创建任务
            TaskArgument taskArgument = this.ConvertToTaskArgument(argumentDict);
            try
            {
                // 检查任务并返回任务
                this.CheckArgumentDictionary(argumentDict);
                return taskArgument;
            }
            catch (Exception ex)
            {
                // 参数错误时附带已创建的任务参数对象并抛出
                ex.Data["TaskArgument"] = taskArgument;
                throw ex;
            }
        }

        /// <summary>
        /// 检查参数字典
        /// </summary>
        /// <param name="argumentDict"></param>
        /// <returns></returns>
        private bool CheckArgumentDictionary(Dictionary<string, string> argumentDict)
        {
            if (!argumentDict.ContainsKey(LOG_DIR))
            {
                throw new ArgumentNullException($"不存在日志文件存放目录参数。参数名称：{LOG_DIR}");
            }

            if (!argumentDict.ContainsKey(MONITOR_NAME))
            {
                throw new ArgumentNullException($"不存在监视规则文件名称参数。参数名称：{MONITOR_NAME}");
            }

            return false;
        }

        /// <summary>
        /// 转换字典为任务参数对象
        /// </summary>
        /// <param name="argumentDict"></param>
        /// <returns></returns>
        private TaskArgument ConvertToTaskArgument(Dictionary<string, string> argumentDict)
        {
            if (argumentDict == null)
            {
                throw new ArgumentNullException(nameof(argumentDict));
            }

            string argumentValue = string.Empty;
            var taskArgument = new TaskArgument();

            if (argumentDict.TryGetValue(LOG_DIR, out argumentValue))
            {
                taskArgument.LogDirectory = argumentValue;
            }

            if (argumentDict.TryGetValue(MONITOR_NAME, out argumentValue))
            {
                taskArgument.MonitorFileName = argumentValue;
            }

            if (argumentDict.TryGetValue(START_TIME, out argumentValue))
            {
                taskArgument.LogStartTime = DateTime.TryParse(argumentValue, out DateTime startTime) ? startTime : DateTime.Today;
            }

            if (argumentDict.TryGetValue(FINISH_TIME, out argumentValue))
            {
                taskArgument.LogFinishTime = DateTime.TryParse(argumentValue, out DateTime finishTime) ? finishTime : DateTime.Now;
            }

            if (argumentDict.TryGetValue(SYS_INFO, out argumentValue))
            {
                taskArgument.IncludeSystemInfo = bool.TryParse(argumentValue, out bool systemInfo) ? systemInfo : false;
            }

            if (argumentDict.TryGetValue(CLIENT_INFO, out argumentValue))
            {
                taskArgument.IncludeClientInfo = bool.TryParse(argumentValue, out bool clientInfo) ? clientInfo : false;
            }

            if (argumentDict.TryGetValue(REPORT_MODE, out argumentValue))
            {
                taskArgument.ReportMode = Enum.TryParse(argumentValue, true, out ReportModes reportModel) ? reportModel : ConfigHelper.DefaultReportMode;
            }

            if (argumentDict.TryGetValue(LOG_LEVEL, out argumentValue))
            {
                ConfigHelper.LogFileLevel = argumentValue;
            }

            if (argumentDict.TryGetValue(AUTO_EXIT, out argumentValue))
            {
                taskArgument.AutoExit = bool.TryParse(argumentValue, out bool autoValue) ? autoValue : false;
            }

            if (argumentDict.TryGetValue(AUTO_OPEN_REPORT, out argumentValue))
            {
                taskArgument.AutoOpenReport = bool.TryParse(argumentValue, out bool autoValue) ? autoValue : true;
            }

            return taskArgument;
        }
    }
}
