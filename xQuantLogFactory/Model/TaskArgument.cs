﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using xQuantLogFactory.Model.Fixed;
using xQuantLogFactory.Model.Monitor;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 任务配置参数
    /// </summary>
    [Serializable]
    [XmlRoot("TaskArgumentRoot")]
    [XmlInclude(typeof(TaskArgument))]
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
        [XmlElement("TaskID")]
        public string TaskID { get; set; }

        /// <summary>
        /// Gets or sets 日志文件目录
        /// </summary>
        [XmlElement("LogDirectory")]
        public string LogDirectory { get; set; }

        /// <summary>
        /// Gets or sets 日志开始时间
        /// </summary>
        [XmlElement("LogStartTime")]
        public DateTime? LogStartTime { get; set; }

        /// <summary>
        /// Gets or sets 日志截止时间
        /// </summary>
        [XmlElement("LogFinishTime")]
        public DateTime? LogFinishTime { get; set; }

        /// <summary>
        /// Gets or sets 任务开始时间
        /// </summary>
        [XmlElement("TaskStartTime")]
        public DateTime TaskStartTime { get; set; }

        /// <summary>
        /// Gets or sets 任务完成时间
        /// </summary>
        [XmlElement("TaskFinishTime")]
        public DateTime TaskFinishTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 包含系统信息
        /// </summary>
        [XmlElement("IncludeSystemInfo")]
        public bool IncludeSystemInfo { get; set; } = false;

        /// <summary>
        /// Gets or sets 系统信息
        /// </summary>
        public SystemInfo SystemInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 包含客户端文件信息
        /// </summary>
        [XmlElement("IncludeClientInfo")]
        public bool IncludeClientInfo { get; set; } = false;

        /// <summary>
        /// Gets or sets 日志分析报告输出模式
        /// </summary>
        [XmlElement("ReportMode")]
        public ReportModes ReportMode { get; set; } = ConfigHelper.DefaultReportMode;

        /// <summary>
        /// Gets or sets 最近一次导出的日志报告路径
        /// </summary>
        [XmlElement("LastReportPath")]
        public string LastReportPath { get; set; }

        /// <summary>
        /// Gets or sets 监视规则文件名称
        /// </summary>
        [XmlElement("MonitorFileName")]
        public string MonitorFileName { get; set; }

        /// <summary>
        /// Gets or sets 监控规则容器
        /// </summary>
        [XmlIgnore]
        public virtual MonitorContainer MonitorContainerRoot { get; set; }

        /// <summary>
        /// Gets or sets 分析结果容器
        /// </summary>
        [XmlIgnore]
        public virtual GroupAnalysisResultContainer AnalysisResultContainerRoot { get; set; } = new GroupAnalysisResultContainer();

        /// <summary>
        /// Gets or sets 日志文件列表
        /// </summary>
        [XmlIgnore]
        public virtual List<LogFile> LogFiles { get; set; } = new List<LogFile>();

        /// <summary>
        /// Gets or sets 监视日志解析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// Gets or sets 中间件日志解析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<MiddlewareResult> MiddlewareResults { get; set; } = new List<MiddlewareResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
        /// </summary>
        [XmlIgnore]
        public virtual List<GroupAnalysisResult> AnalysisResults { get; set; } = new List<GroupAnalysisResult>();

        /// <summary>
        /// 初始化分析结果树
        /// </summary>
        public void InitAnalysisResultTree()
        {
            this.AnalysisResultContainerRoot.InitAnalysisResultTree(this.AnalysisResults);
        }

        public override string ToString()
        {
            return $"\t日志文件目录：{this.LogDirectory}\n\t含客户端信息：{this.IncludeClientInfo}\n\t包含系统信息：{this.IncludeSystemInfo}\n\t监视规则文件：{(string.IsNullOrEmpty(this.MonitorFileName) ? "[全部规则文件]" : this.MonitorFileName)}\n\t日志开始时间：{this.LogStartTime?.ToString() ?? "[不限制]"}\n\t日志截止时间：{this.LogFinishTime?.ToString() ?? "[不限制]"}\n\t报告导出格式：{this.ReportMode.ToString()}\n\t任务执行时间：{this.TaskStartTime}";
        }
    }
}
