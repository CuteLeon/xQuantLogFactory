using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils;

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
        public TaskArgument()
        {
            this.TaskID = Guid.NewGuid().ToString("N");
            this.TaskStartTime = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets 任务ID
        /// </summary>
        [Key]
        [Required]
        [XmlElement("TaskID")]
        [DisplayName("任务ID")]
        [DataType(DataType.Text)]
        public string TaskID { get; set; }

        /// <summary>
        /// Gets or sets 日志文件目录
        /// </summary>
        [Required]
        [XmlElement("LogDirectory")]
        [DisplayName("日志文件目录")]
        [DataType(DataType.Text)]
        public string LogDirectory { get; set; }

        /// <summary>
        /// Gets or sets 日志开始时间
        /// </summary>
        [XmlElement("LogStartTime")]
        [DisplayName("日志开始时间")]
        [DataType(DataType.DateTime)]
        public DateTime? LogStartTime { get; set; }

        /// <summary>
        /// Gets or sets 日志截止时间
        /// </summary>
        [XmlElement("LogFinishTime")]
        [DisplayName("日志截止时间")]
        [DataType(DataType.DateTime)]
        public DateTime? LogFinishTime { get; set; }

        /// <summary>
        /// Gets or sets 任务开始时间
        /// </summary>
        [Required]
        [XmlElement("TaskStartTime")]
        [DisplayName("任务开始时间")]
        [DataType(DataType.DateTime)]
        public DateTime TaskStartTime { get; set; }

        /// <summary>
        /// Gets or sets 任务完成时间
        /// </summary>
        [XmlElement("TaskFinishTime")]
        [DisplayName("任务完成时间")]
        [DataType(DataType.DateTime)]
        public DateTime TaskFinishTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 包含系统信息
        /// </summary>
        [Required]
        [XmlElement("IncludeSystemInfo")]
        [DisplayName("包含系统信息")]
        public bool IncludeSystemInfo { get; set; } = false;

        /// <summary>
        /// Gets or sets 系统信息
        /// </summary>
        [DisplayName("系统信息")]
        public SystemInfo SystemInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 包含客户端文件信息
        /// </summary>
        [Required]
        [XmlElement("IncludeClientInfo")]
        [DisplayName("包含客户端文件信息")]
        public bool IncludeClientInfo { get; set; } = false;

        /// <summary>
        /// Gets or sets 日志分析报告输出模式
        /// </summary>
        [Required]
        [XmlElement("ReportMode")]
        [DisplayName("日志分析报告输出模式")]
        public ReportModes ReportMode { get; set; } = ConfigHelper.DefaultReportMode;

        /// <summary>
        /// Gets or sets 最近一次导出的日志报告路径
        /// </summary>
        [XmlElement("LastReportPath")]
        [DisplayName("最近一次导出的日志报告路径")]
        [DataType(DataType.Text)]
        public string LastReportPath { get; set; }

        /// <summary>
        /// Gets or sets 监视规则文件名称
        /// </summary>
        [Required]
        [XmlElement("MonitorFileName")]
        [DisplayName("时间规则文件名称")]
        [DataType(DataType.Text)]
        public string MonitorFileName { get; set; }

        /// <summary>
        /// Gets or sets 监控规则容器
        /// </summary>
        [XmlIgnore]
        [DisplayName("监控规则容器")]
        public virtual MonitorContainer MonitorContainerRoot { get; set; }

        /// <summary>
        /// Gets or sets 日志文件列表
        /// </summary>
        [XmlIgnore]
        [DisplayName("日志文件列表")]
        public virtual List<LogFile> LogFiles { get; set; } = new List<LogFile>();

        /// <summary>
        /// Gets or sets 监视日志解析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("监视日志解析结果表")]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// Gets or sets 中间件日志解析结果表
        /// </summary>
        [XmlIgnore]
        [DisplayName("中间件日志解析结果表")]
        public virtual List<MiddlewareResult> MiddlewareResults { get; set; } = new List<MiddlewareResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
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
