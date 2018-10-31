using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using xQuantLogFactory.DAL;

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
    [Table("Tasks")]
    public class TaskArgument
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("任务ID"), DataType(DataType.Text)]
        public string TaskID { get; set; }

        /// <summary>
        /// 日志文件目录
        /// </summary>
        [Required]
        [DisplayName("日志文件目录"), DataType(DataType.Text)]
        public string BaseDirectory { get; set; }

        /// <summary>
        /// 日志开始时间
        /// </summary>
        [Required]
        [DisplayName("日志开始时间"), DataType(DataType.DateTime)]
        public DateTime LogStartTime { get; set; }

        /// <summary>
        /// 日志截止时间
        /// </summary>
        [Required]
        [DisplayName("日志截止时间"), DataType(DataType.DateTime)]
        public DateTime LogFinishTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [Required]
        [DisplayName("任务开始时间"), DataType(DataType.DateTime)]
        public DateTime TaskStartTime { get; set; }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        [DisplayName("任务完成时间"), DataType(DataType.DateTime)]
        public DateTime TaskFinishTime { get; set; }

        /// <summary>
        /// 包含系统信息
        /// </summary>
        [Required]
        [DisplayName("包含系统信息"), DataType(DataType.DateTime)]
        public bool IncludeSystemInfo { get; set; } = false;

        /// <summary>
        /// 包含客户端文件信息
        /// </summary>
        [Required]
        [DisplayName("包含客户端文件信息")]
        public bool IncludeClientInfo { get; set; } = false;

        /// <summary>
        /// 日志分析报告输出模式
        /// </summary>
        [Required]
        [DisplayName("日志分析报告输出模式")]
        public ReportModes ReportMode { get; set; } = ReportModes.HTML;

        /// <summary>
        /// 监控的项目名称列表
        /// </summary>
        [Required]
        [DisplayName("监控的项目名称列表")]
        public virtual List<string> ItemNames { get; set; } = new List<string>();

        /// <summary>
        /// 监控规则列表
        /// </summary>
        [DisplayName("监控规则列表")]
        public virtual List<MonitorItem> MonitorItems { get; set; } = new List<MonitorItem>();

        /// <summary>
        /// 日志文件列表
        /// </summary>
        [DisplayName("日志文件列表")]
        public virtual List<LogFile> LogFiles { get; set; } = new List<LogFile>();

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
                TaskID = Guid.NewGuid().ToString("N"),
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
