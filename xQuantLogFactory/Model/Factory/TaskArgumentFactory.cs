using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace xQuantLogFactory.Model.Factory
{

    /// <summary>
    /// 任务参数对象工厂
    /// </summary>
    public class TaskArgumentFactory
    {
        /*
         * logdir={string_日志文件目录}：目录含有空格时需要在值外嵌套英文双引号；如：C:\TEST_DIR 或 "C:\TEST DIR" 
         * monitor={string.Format(,)_监控的项目名称列表}：可省略，默认为所有监视规则；程序监控的项目名称列表；当存在多个值时，值间以英文逗号分隔(不加空格)；含有空格时需要在值外嵌套英文双引号，如：监控项目_Demo 或 监控项目_0,监控项目_1 或 "监控项目_0,监控项目 1"
         * start={datetime_日志开始时间}：可省略，以格式化日期时间传入；采用24小时制；格式如：yyyy-MM-dd HH:mm:ss
         * finish={datetime_日志截止时间 =DateTime.Now}：可省略，默认值为当前时间；格式同日志开始时间；采用24小时制；
         * sysinfo={boolean_包含系统信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * clntinfo={boolean_包含客户端信息 =false}：可省略，默认值为 false；可取值为：{false/true}，可忽略大小写
         * report={reportmodes_报告导出模式 =RepostModes.Html}：可省略，默认值为 Html；可取值为：{html/word/excel}，可忽略大小写
         */

        private static Lazy<TaskArgumentFactory> factory = new Lazy<TaskArgumentFactory>();
        /// <summary>
        /// 任务参数工厂实例
        /// </summary>
        public static TaskArgumentFactory Intance
        {
            get => factory.Value;
        }

        private static Lazy<Dictionary<string, string>> argumentDescription = new Lazy<Dictionary<string, string>>();
        /// <summary>
        /// 参数描述
        /// </summary>
        private static Dictionary<string, string> ArgumentDescriptions
        {
            get
            {
                if (!argumentDescription.IsValueCreated)
                {
                    argumentDescription.Value.Add("logdir", "日志文件存放目录，如：D:\\Log 或 \"E:\\Log Dir\"");
                    argumentDescription.Value.Add("monitor", "监视规则文件名称，如：monitor.xml");
                    argumentDescription.Value.Add("start", "日志开始时间，如：\"2018-10-01 17:30:00\"");
                    argumentDescription.Value.Add("finish", "日志结束时间，如：\"2018-11-11 08:40:00\"");
                    argumentDescription.Value.Add("sysinfo", "是否记录系统信息，如：true 或 false");
                    argumentDescription.Value.Add("cltinfo", "是否记录客户端信息，如：true 或 false");
                    argumentDescription.Value.Add("report", "导出报告模式，如：excel 或 html 或 word");
                }

                return argumentDescription.Value;
            }
        }

        private static readonly Lazy<StringBuilder> usageBuilder = new Lazy<StringBuilder>();
        /// <summary>
        /// 参数使用方法
        /// </summary>
        public string Usage
        {
            get
            {
                if (!usageBuilder.IsValueCreated)
                {
                    usageBuilder.Value.AppendLine($"{Console.Title} 命令参数说明：");
                    usageBuilder.Value.AppendLine();

                    usageBuilder.Value.AppendLine("参数格式规范：");
                    usageBuilder.Value.AppendLine("\t1.参数传入格式：[参数名称]=[参数数据]");
                    usageBuilder.Value.AppendLine("\t2.参数名称和数据内存在空格时，需要在外面嵌套英文双引号");
                    usageBuilder.Value.AppendLine("\t3.参数名称可忽略大小写");
                    usageBuilder.Value.AppendLine();

                    usageBuilder.Value.AppendLine("参数说明：");
                    usageBuilder.Value.AppendLine("\t[名称]\t\t[描述]");
                    foreach (var arg in ArgumentDescriptions)
                        usageBuilder.Value.AppendLine($"\t{arg.Key}\t\t{arg.Value}");
                }

                return usageBuilder.Value.ToString();
            }
        }

        /// <summary>
        /// 参数字典
        /// </summary>
        private readonly Dictionary<string, string> argumentDictionary = new Dictionary<string, string>();

        private static readonly Regex argRegex = new Regex(@"(?<ArgName>.*)=(?<ArgValue>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <param name="args">工具启动参数</param>
        /// <returns>任务参数对象</returns>
        public TaskArgument CreateTaskArgument(string[] args)
        {
            if (args == null || args.Length == 0)
                throw new ArgumentNullException(nameof(args));

            //解析参数并录入字典
            Match argMatch = null;
            foreach (var arg in args)
            {
                argMatch = argRegex.Match(arg);
                if (argMatch.Success &&
                    argMatch.Groups["ArgName"].Success &&
                    argMatch.Groups["ArgValue"].Success
                    )
                {
                    this.argumentDictionary[argMatch.Groups["ArgName"].Value] = argMatch.Groups["ArgValue"].Value;
                }
            }

            //检查必选字段
            if (!this.argumentDictionary.ContainsKey("logdir"))
            {
                throw new ArgumentNullException("不存在日志文件存放目录参数。");
            }

            //根据字典创建任务参数对象
            return this.ConvertTaskArgument(this.argumentDictionary);
        }

        /// <summary>
        /// 转换字典为任务参数对象
        /// </summary>
        /// <param name="argumentDic"></param>
        /// <returns></returns>
        private TaskArgument ConvertTaskArgument(Dictionary<string, string> argumentDic)
        {
            if (argumentDic == null)
                throw new ArgumentNullException(nameof(argumentDic));

            string argumentValue = string.Empty;
            var taskArgument = new TaskArgument
            {
                TaskID = Guid.NewGuid().ToString("N"),
                TaskStartTime = DateTime.Now,
            };

            if (this.argumentDictionary.TryGetValue("logdir", out argumentValue))
                taskArgument.LogDirectory = argumentValue;

            if (this.argumentDictionary.TryGetValue("monitor", out argumentValue))
                taskArgument.MonitorItemNames.AddRange(argumentValue.Split(','));

            if (this.argumentDictionary.TryGetValue("start", out argumentValue))
                taskArgument.LogStartTime = DateTime.TryParse(argumentValue, out DateTime startTime) ? startTime : DateTime.Today;

            if (this.argumentDictionary.TryGetValue("finish", out argumentValue))
                taskArgument.LogFinishTime = DateTime.TryParse(argumentValue, out DateTime finishTime) ? finishTime : DateTime.Now;

            if (this.argumentDictionary.TryGetValue("sysinfo", out argumentValue))
                taskArgument.IncludeSystemInfo = bool.TryParse(argumentValue, out bool systemInfo) ? systemInfo : false;

            if (this.argumentDictionary.TryGetValue("clntinfo", out argumentValue))
                taskArgument.IncludeClientInfo = bool.TryParse(argumentValue, out bool clientInfo) ? clientInfo : false;

            if (this.argumentDictionary.TryGetValue("report", out argumentValue))
                taskArgument.ReportMode = Enum.TryParse(argumentValue, out ReportModes reportModel) ? reportModel : ReportModes.HTML;

            return taskArgument;
        }

    }
}
