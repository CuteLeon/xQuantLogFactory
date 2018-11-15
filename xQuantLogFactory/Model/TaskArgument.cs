using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils;
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
        [AmbientValue("xlsx")]
        Excel = 3
    }

    /// <summary>
    /// 任务配置参数
    /// </summary>
    [Serializable]
    [XmlRoot("TaskArgumentRoot")]
    [XmlInclude(typeof(TaskArgument))]
    [Table("TaskArguments")]
    public class TaskArgument
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [Key]
        [Required]
        [XmlElement("TaskID")]
        [DisplayName("任务ID"), DataType(DataType.Text)]
        public string TaskID { get; set; }

        /// <summary>
        /// 日志文件目录
        /// </summary>
        [Required]
        [XmlElement("LogDirectory")]
        [DisplayName("日志文件目录"), DataType(DataType.Text)]
        public string LogDirectory { get; set; }

        /// <summary>
        /// 日志开始时间
        /// </summary>
        [XmlElement("LogStartTime")]
        [DisplayName("日志开始时间"), DataType(DataType.DateTime)]
        public DateTime? LogStartTime { get; set; }

        /// <summary>
        /// 日志截止时间
        /// </summary>
        [XmlElement("LogFinishTime")]
        [DisplayName("日志截止时间"), DataType(DataType.DateTime)]
        public DateTime? LogFinishTime { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [Required]
        [XmlElement("TaskStartTime")]
        [DisplayName("任务开始时间"), DataType(DataType.DateTime)]
        public DateTime TaskStartTime { get; set; }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        [XmlElement("TaskFinishTime")]
        [DisplayName("任务完成时间"), DataType(DataType.DateTime)]
        public DateTime TaskFinishTime { get; set; }

        /// <summary>
        /// 包含系统信息
        /// </summary>
        [Required]
        [XmlElement("IncludeSystemInfo")]
        [DisplayName("包含系统信息")]
        public bool IncludeSystemInfo { get; set; } = false;

        /// <summary>
        /// 系统信息
        /// </summary>
        [DisplayName("系统信息")]
        public SystemInfo SystemInfo { get; set; }

        /// <summary>
        /// 包含客户端文件信息
        /// </summary>
        [Required]
        [XmlElement("IncludeClientInfo")]
        [DisplayName("包含客户端文件信息")]
        public bool IncludeClientInfo { get; set; } = false;

        /// <summary>
        /// 日志分析报告输出模式
        /// </summary>
        [Required]
        [XmlElement("ReportMode")]
        [DisplayName("日志分析报告输出模式")]
        public ReportModes ReportMode { get; set; } = ConfigHelper.DefaultReportMode;

        /// <summary>
        /// 最近一次导出的日志报告路径
        /// </summary>
        [XmlElement("LastReportPath")]
        [DisplayName("最近一次导出的日志报告路径"), DataType(DataType.Text)]
        public string LastReportPath { get; set; }

        /// <summary>
        /// 监视规则文件名称
        /// </summary>
        [Required]
        [XmlElement("MonitorFileName")]
        [DisplayName("时间规则文件名称"), DataType(DataType.Text)]
        public string MonitorFileName { get; set; }

        /// <summary>
        /// 子监控项目树根节点列表
        /// </summary>
        [XmlIgnore]
        [DisplayName("子监控项目树根节点列表")]
        public virtual VersionedList<MonitorItem> MonitorItemTree { get; set; } = new VersionedList<MonitorItem>();

        private readonly VersionedList<MonitorItem> monitorItems = new VersionedList<MonitorItem>();
        /// <summary>
        /// 子监控项目遍历列表
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        [DisplayName("子监控项目遍历列表"), DataType(DataType.Duration)]
        public virtual List<MonitorItem> MonitorItems
        {
            get
            {
                //二维列表版本号低于树状列表时，更新二维列表
                if (this.monitorItems.Version == 0 || //EF初始化 MonitorItemTree 时版本号不会自增
                    this.monitorItems.Version != this.MonitorItemTree.Version
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
                this.MonitorItemTree.ForEach(monitorRoot =>
                    {
                        //深度优先
                        Stack<IMonitor> parentStack = new Stack<IMonitor>();
                        parentStack.Push(monitorRoot);
                        this.monitorItems.Add(monitorRoot);

                        //当前父节点指针
                        IMonitor currentMonitor = null;
                        while (parentStack.Count > 0)
                        {
                            currentMonitor = parentStack.Pop();

                            currentMonitor.MonitorItems.Where(monitor => monitor.HasChildren).ToList().ForEach(monitor => parentStack.Push(monitor));
                            this.monitorItems.AddRange(currentMonitor.MonitorItems);
                        }
                    });
            }

            //同步完成后更新一次版本号，防止版本号一直为0而频繁刷新浪费性能
            this.MonitorItemTree.UpdateVersion();
            //同步二维列表版本号
            this.monitorItems.SynchronizeVersion(this.MonitorItemTree);
        }

        /// <summary>
        /// 日志文件列表
        /// </summary>
        [XmlIgnore]
        [DisplayName("日志文件列表")]
        public virtual List<LogFile> LogFiles { get; set; } = new List<LogFile>();

        /// <summary>
        /// 监视日志解析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("监视日志解析结果表")]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// 中间件日志解析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("中间件日志解析结果表")]
        public virtual List<MiddlewareResult> MiddlewareResults { get; set; } = new List<MiddlewareResult>();

        /// <summary>
        /// 日志分析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("日志分析结果表")]
        public virtual List<GroupAnalysisResult> AnalysisResults { get; set; } = new List<GroupAnalysisResult>();

        public override string ToString()
        {
            return $"\t日志文件目录：{this.LogDirectory}\n\t含客户端信息：{this.IncludeClientInfo}\n\t包含系统信息：{this.IncludeSystemInfo}\n\t监视规则文件：{(string.IsNullOrEmpty(this.MonitorFileName) ? this.MonitorFileName : "[全部规则文件]")}\n\t日志开始时间：{this.LogStartTime?.ToString() ?? "[不限制]"}\n\t日志截止时间：{this.LogFinishTime?.ToString() ?? "[不限制]"}\n\t报告导出格式：{this.ReportMode.ToString()}\n\t任务执行时间：{this.TaskStartTime}";
        }

    }
}
