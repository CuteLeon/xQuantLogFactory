using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using xQuantLogFactory.Model.Result;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 日志文件类型
    /// </summary>
    public enum LogFileTypes
    {
        /// <summary>
        /// 客户端日志文件
        /// </summary>
        Client = 1,

        /// <summary>
        /// 服务端日志文件
        /// </summary>
        Server = 2,

        /// <summary>
        /// 中间件日志
        /// </summary>
        Middleware = 3,
    }

    /// <summary>
    /// 日志文件
    /// </summary>
    [Table("LogFiles")]
    public class LogFile
    {
        public LogFile()
        {
        }

        public LogFile(LogFileTypes logFileTypes, string filePath, DateTime createTime, DateTime lastWriteTime, string relativePath)
        {
            this.LogFileType = logFileTypes;
            this.FilePath = filePath;
            this.CreateTime = createTime;
            this.LastWriteTime = lastWriteTime;
            this.RelativePath = relativePath;
        }

        /// <summary>
        /// Gets or sets 日志文件ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("日志文件ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileID { get; set; }

        /// <summary>
        /// Gets or sets 日志文件路径
        /// </summary>
        [Required]
        [DisplayName("日志文件路径")]
        [DataType(DataType.Text)]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件相对路径
        /// </summary>
        [Required]
        [DisplayName("日志文件相对路径")]
        [DataType(DataType.Text)]
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets 日志文件类型
        /// </summary>
        [DisplayName("日志文件类型")]
        public LogFileTypes LogFileType { get; set; }

        /// <summary>
        /// Gets or sets 文件创建时间
        /// </summary>
        [Required]
        [DisplayName("文件创建时间")]
        [DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets 上次写入时间
        /// </summary>
        [Required]
        [DisplayName("上次写入时间")]
        [DataType(DataType.DateTime)]
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// Gets or sets 客户端和服务端匹配结果总耗时（单位：毫秒）
        /// </summary>
        [DisplayName("客户端和服务端匹配结果总耗时（单位：毫秒）")]
        [DataType(DataType.Duration)]
        public double ElapsedMillisecond { get; set; }

        /// <summary>
        /// Gets or sets 监视日志解析结果列表
        /// </summary>
        [DisplayName("监视日志解析结果列表")]
        public virtual List<MonitorResult> MonitorResults { get; set; } = new List<MonitorResult>();

        /// <summary>
        /// Gets or sets 中间件日志解析结果表
        /// </summary>
        [DisplayName("中间件日志解析结果表")]
        public virtual List<MiddlewareResult> MiddlewareResults { get; set; } = new List<MiddlewareResult>();

        /// <summary>
        /// Gets or sets 日志分析结果表
        /// </summary>
        [DisplayName("日志分析结果表")]
        public virtual List<GroupAnalysisResult> AnalysisResults { get; set; } = new List<GroupAnalysisResult>();
    }
}
