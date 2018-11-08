using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;

using xQuantLogFactory.Utils.Collections;

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
        [AmbientValue("html")]
        HTML = 1,

        /// <summary>
        /// 生成Word
        /// </summary>
        [AmbientValue("doc")]
        Word = 2,

        /// <summary>
        /// 生成Excel
        /// </summary>
        [AmbientValue("xls")]
        Excel = 3
    }

    /// <summary>
    /// 任务配置参数
    /// </summary>
    [Serializable]
    [XmlRoot("TaskArgumentRoot")]
    [Table("TaskArguments")]
    public class TaskArgument
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [Key]
        [Required]
        [XmlAttribute("TaskID")]
        [DisplayName("任务ID"), DataType(DataType.Text)]
        public string TaskID { get; set; }

        /// <summary>
        /// 日志文件目录
        /// </summary>
        [Required]
        [XmlAttribute("LogDirectory")]
        [DisplayName("日志文件目录"), DataType(DataType.Text)]
        public string LogDirectory { get; set; }

        /// <summary>
        /// 是否筛选日志时间
        /// </summary>
        [NotMapped]
        public bool CheckLogTime
        {
            get => this.LogStartTime != null && this.LogFinishTime != null;
        }

        /// <summary>
        /// 日志开始时间
        /// </summary>
        [XmlAttribute("LogStartTime")]
        [DisplayName("日志开始时间"), DataType(DataType.DateTime)]
        public DateTime? LogStartTime { get; set; }

        /// <summary>
        /// 日志截止时间
        /// </summary>
        [XmlAttribute("LogFinishTime")]
        [DisplayName("日志截止时间"), DataType(DataType.DateTime)]
        public DateTime? LogFinishTime { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [Required]
        [XmlAttribute("TaskStartTime")]
        [DisplayName("任务开始时间"), DataType(DataType.DateTime)]
        public DateTime TaskStartTime { get; set; }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        [XmlAttribute("TaskFinishTime")]
        [DisplayName("任务完成时间"), DataType(DataType.DateTime)]
        public DateTime TaskFinishTime { get; set; }

        /// <summary>
        /// 包含系统信息
        /// </summary>
        [Required]
        [XmlAttribute("IncludeSystemInfo")]
        [DisplayName("包含系统信息")]
        public bool IncludeSystemInfo { get; set; } = false;

        /// <summary>
        /// 系统信息
        /// </summary>
        [XmlAttribute("SystemInfo")]
        [DisplayName("系统信息")]
        public SystemInfo SystemInfo { get; set; }

        /// <summary>
        /// 包含客户端文件信息
        /// </summary>
        [Required]
        [XmlAttribute("IncludeClientInfo")]
        [DisplayName("包含客户端文件信息")]
        public bool IncludeClientInfo { get; set; } = false;

        /// <summary>
        /// 日志分析报告输出模式
        /// </summary>
        [Required]
        [XmlAttribute("ReportMode")]
        [DisplayName("日志分析报告输出模式")]
        public ReportModes ReportMode { get; set; } = ReportModes.Excel;

        /// <summary>
        /// 最近一次导出的日志报告路径
        /// </summary>
        [XmlAttribute("LastReportPath")]
        [DisplayName("最近一次导出的日志报告路径"), DataType(DataType.Text)]
        public string LastReportPath { get; set; }

        /// <summary>
        /// 监控的项目名称列表
        /// </summary>
        [Required]
        [DisplayName("监控的项目名称列表")]
        public virtual List<string> MonitorItemNames { get; set; } = new List<string>();

        /// <summary>
        /// 子监控项目树根节点列表
        /// </summary>
        [DisplayName("子监控项目树根节点列表")]
        public virtual VersionedList<MonitorItem> MonitorItemTree { get; set; } = new VersionedList<MonitorItem>();

        [XmlIgnore]
        private readonly VersionedList<MonitorItem> monitorItems = new VersionedList<MonitorItem>();
        /// <summary>
        /// 子监控项目前序遍历列表
        /// </summary>
        /// <remarks>树状结构更新时需要手动刷新二维列表 this.RefreshMonitorItems()</remarks>
        [XmlIgnore]
        [NotMapped]
        [DisplayName("子监控项目前序遍历列表"), DataType(DataType.Duration)]
        public virtual List<MonitorItem> MonitorItems
        {
            get
            {
                //二维列表版本号低于树状列表时，更新二维列表
                if (this.monitorItems.Version == 0 || //EF初始化 MonitorItemTree 时版本号不会自增
                    this.monitorItems.Version < this.MonitorItemTree.Version
                    )
                    this.RefreshMonitorItems();

                return this.monitorItems;
            }
        }

        /// <summary>
        /// 刷新监视规则树状结构至二维列表
        /// </summary>
        protected void RefreshMonitorItems()
        {
            this.monitorItems.Clear();

            if (this.MonitorItemTree.Count > 0)
            {
                Queue<MonitorItem> parentList = new Queue<MonitorItem>();

                this.monitorItems.AddRange(this.MonitorItemTree);
                this.MonitorItemTree.Where(monitor => monitor.HasChildren).ToList().ForEach(monitor => parentList.Enqueue(monitor));

                //当前父节点指针
                MonitorItem currentMonitor = null;
                while (parentList.Count > 0)
                {
                    currentMonitor = parentList.Dequeue();

                    this.monitorItems.AddRange(currentMonitor.MonitorItems);
                    currentMonitor.MonitorItems.Where(monitor => monitor.HasChildren).ToList().ForEach(monitor => parentList.Enqueue(monitor));
                }
            }

            //同步完成后更新一次版本号，防止版本号一直为0而频繁刷新浪费性能
            this.MonitorItemTree.UpdateVersion();
            //同步二维列表版本号
            this.monitorItems.SynchronizeVersion(this.MonitorItemTree);
        }

        /// <summary>
        /// 日志文件列表
        /// </summary>
        [DisplayName("日志文件列表")]
        public virtual List<LogFile> LogFiles { get; set; } = new List<LogFile>();

        /// <summary>
        /// 监视日志解析结果表
        /// </summary>
        [DisplayName("监视日志解析结果表")]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// 中间件日志解析结果表
        /// </summary>
        [DisplayName("中间件日志解析结果表")]
        public virtual List<MiddlewareResult> MiddlewareResults { get; set; } = new List<MiddlewareResult>();

        /// <summary>
        /// 日志分析结果表
        /// </summary>
        [DisplayName("日志分析结果表")]
        public virtual List<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();

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
            6 [reportmodes_报告导出模式 = RepostModes.HTML]
             */
            if (args.Length == 0)
                throw new ArgumentOutOfRangeException("不存在启动命令行参数，请至少输入参数：日志文件存放目录！ ");

            //基础参数
            var argument = new TaskArgument
            {
                TaskID = Guid.NewGuid().ToString("N"),
                TaskStartTime = DateTime.Now,
                LogDirectory = args[0],
            };

            //可选参数
            if (args.Length >= 2)
            {
                argument.MonitorItemNames.AddRange(args[1].Split(','));
            }
            else
            {
                return argument;
            }

            if (args.Length >= 3)
            {
                argument.LogStartTime = DateTime.Parse(args[2]);
            }
            else
            {
                return argument;
            }

            if (args.Length >= 4)
            {
                if (argument.LogStartTime == null)
                {
                    argument.LogFinishTime = null;
                }
                else
                {
                    argument.LogFinishTime = DateTime.TryParse(args[3], out DateTime finishTime) ? finishTime : DateTime.Now;
                }
            }
            else
            {
                return argument;
            }

            if (args.Length >= 5)
            {
                argument.IncludeSystemInfo = bool.TryParse(args[4], out bool systemInfo) ? systemInfo : false;
            }
            else
            {
                return argument;
            }

            if (args.Length >= 6)
            {
                argument.IncludeClientInfo = bool.TryParse(args[5], out bool clientInfo) ? clientInfo : false;
            }
            else
            {
                return argument;
            }

            if (args.Length >= 7)
            {
                argument.ReportMode = Enum.TryParse(args[6], out ReportModes reportModel) ? reportModel : ReportModes.HTML;
            }
            else
            {
                return argument;
            }

            return argument;
        }

        public override string ToString()
        {
            return $"\t日志文件目录：{this.LogDirectory}\n\t含客户端信息：{this.IncludeClientInfo}\n\t包含系统信息：{this.IncludeSystemInfo}\n\t监视项目列表：{(this.MonitorItemNames.Count > 0 ? string.Join("、", this.MonitorItemNames) : "[全部规则]")}\n\t日志开始时间：{this.LogStartTime?.ToString() ?? "[不限制]"}\n\t日志截止时间：{this.LogFinishTime?.ToString() ?? "[不限制]"}\n\t报告导出格式：{this.ReportMode.ToString()}\n\t任务执行时间：{this.TaskStartTime}";
        }

    }
}
