using System;
using System.Collections.Generic;

namespace xQuantLogFactory.Model
{

    /// <summary>
    /// 日志分析报告输出模式
    /// </summary>
    public enum ReportModes
    {
        /// <summary>
        /// 生成HTML
        /// </summary>
        HTML = 1,

        /// <summary>
        /// 生成Word
        /// </summary>
        Word = 2,

        /// <summary>
        /// 生成Excel
        /// </summary>
        Excel = 3
    }

    /// <summary>
    /// 任务配置参数
    /// </summary>
    public class TaskArgument
    {
        /// <summary>
        /// 日志文件目录
        /// </summary>
        public string BaseDirectory { get; protected set; }

        /// <summary>
        /// 日志开始时间
        /// </summary>
        public DateTime LogStartTime { get; protected set; }

        /// <summary>
        /// 日志截止时间
        /// </summary>
        public DateTime LogFinishTime { get; protected set; } = DateTime.Now;

        /// <summary>
        /// 任务开始时间
        /// </summary>
        public DateTime TaskStartTime { get; set; }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        public DateTime TaskFinishTime { get; set; }

        /// <summary>
        /// 包含系统信息
        /// </summary>
        public bool IncludeSystemInfo { get; protected set; } = false;

        /// <summary>
        /// 包含客户端文件信息
        /// </summary>
        public bool IncludeClientInfo { get; protected set; } = false;

        /// <summary>
        /// 日志分析报告输出模式
        /// </summary>
        public ReportModes ReportMode { get; protected set; } = ReportModes.HTML;

        /// <summary>
        /// 监控的项目名称列表
        /// </summary>
        public List<string> ItemNames { get; protected set; } = new List<string>();

        /// <summary>
        /// 根据工具启动参数创建任务参数对象
        /// </summary>
        /// <param name="args">工具启动参数</param>
        /// <returns>任务参数对象</returns>
        public static TaskArgument Parse(string[] args)
        {
            /*
            0 {string_日志文件目录}
            1 {string.Format(,)_监控的项目名称列表}
            2 "{datetime_日志开始时间}"
            3 "[datetime_日志截止时间 = DateTime.Now]"
            4 [boolean_包含系统信息 = false]
            5 [boolean_包含客户端信息 = false]
            6 [reportmodes_报告导出模式 = RepostModes.Html]
             */
            if (args.Length < 3 || args.Length > 7)
                throw new ArgumentOutOfRangeException("启动参数长度应为 3~7");

            //基础参数
            var argument = new TaskArgument
            {
                TaskStartTime = DateTime.Now,
                BaseDirectory = args[0],
                LogStartTime = DateTime.Parse(args[2]),
            };
            argument.ItemNames.AddRange(args[1].Split(','));

            //可选参数
            if (args.Length >= 4)
                argument.LogFinishTime = DateTime.TryParse(args[3], out DateTime finishTime) ? finishTime : DateTime.Now;
            else
                return argument;

            if (args.Length >= 5)
                argument.IncludeSystemInfo = bool.TryParse(args[4], out bool systemInfo) ? systemInfo : false;
            else
                return argument;

            if (args.Length >= 6)
                argument.IncludeClientInfo = bool.TryParse(args[5], out bool clientInfo) ? clientInfo : false;
            else
                return argument;

            if (args.Length >= 7)
                argument.ReportMode = Enum.TryParse(args[6], out ReportModes reportModel) ? reportModel : ReportModes.HTML;
            else
                return argument;

            return argument;
        }

        public override string ToString()
        {
            return $"\t日志文件目录：{this.BaseDirectory}\n\t含客户端信息：{this.IncludeClientInfo}\n\t包含系统信息：{this.IncludeSystemInfo}\n\t监视项目列表：{string.Join("、", this.ItemNames)}\n\t日志开始时间：{this.LogStartTime}\n\t日志截止时间：{this.LogFinishTime}\n\t报告导出格式：{this.ReportMode.ToString()}\n\t任务执行时间：{this.TaskStartTime}";
        }

    }
}
