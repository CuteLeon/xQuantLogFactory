using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 监视规则解析结果
    /// </summary>
    [Table("MonitorResults")]
    public class MonitorResult
    {
        /// <summary>
        /// 解析结果ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("解析结果ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultID { get; set; }

        /// <summary>
        /// 日志文件
        /// </summary>
        [Required]
        [DisplayName("日志文件")]
        public LogFile LogFile { get; set; }

        /// <summary>
        /// 日志写入时间
        /// </summary>
        [Required]
        [DisplayName("日志写入时间"), DataType(DataType.DateTime)]
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 监控项目
        /// </summary>
        [Required]
        [DisplayName("监控项目")]
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// 匹配模式
        /// </summary>
        [Required]
        [DisplayName("匹配模式"), DataType(DataType.Text)]
        public string Pattern { get; set; }

        /// <summary>
        /// 日志文件中的行号
        /// </summary>
        [Required]
        [DisplayName("日志文件中的行号")]
        public int LineNumber { get; set; }

        /// <summary>
        /// 程序版本
        /// </summary>
        [DisplayName("程序版本"), DataType(DataType.Text)]
        public string Version { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [DisplayName("summary"), DataType(DataType.Text)]
        public string IPAddress { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        [Required]
        [DisplayName("日志级别"), DataType(DataType.Text)]
        public string Level { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [Required]
        [DisplayName("日志内容"), DataType(DataType.Text)]
        public string LogContent { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", this.LogFile, this.Pattern, this.LogTime, this.LogContent);
        }
    }
}
