using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        Server =2,
    }

    /// <summary>
    /// 日志文件
    /// </summary>
    [Table("LogFiles")]
    public class LogFile
    {
        /// <summary>
        /// 日志文件ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("日志文件ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileID { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        [Required]
        [DisplayName("日志文件路径"), DataType(DataType.Text)]
        public string FilePath { get; set; }

        /// <summary>
        /// 日志文件类型
        /// </summary>
        [DisplayName("日志文件类型")]
        public LogFileTypes LogFileType { get; set; }

        /// <summary>
        /// 文件创建时间
        /// </summary>
        [Required]
        [DisplayName("文件创建时间"), DataType(DataType.DateTime)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 上次写入时间
        /// </summary>
        [Required]
        [DisplayName("上次写入时间"), DataType(DataType.DateTime)]
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// 解析结果列表
        /// </summary>
        [DisplayName("解析结果列表")]
        public virtual List<MonitorResult> MonitorResults { get; set; }

    }
}
