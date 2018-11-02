using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 结果模式
    /// </summary>
    public enum ResultTypes
    {
        /// <summary>
        /// 未匹配
        /// </summary>
        Unmatch = 0,
        /// <summary>
        /// 开始匹配
        /// </summary>
        Start = 1,
        /// <summary>
        /// 结束匹配
        /// </summary>
        Finish = 2,
    }

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
        /// 任务参数
        /// </summary>
        [Required]
        [DisplayName("任务参数")]
        public TaskArgument TaskArgument { get; set; }

        /// <summary>
        /// 日志文件
        /// </summary>
        [Required]
        [DisplayName("日志文件")]
        public LogFile LogFile { get; set; }

        /// <summary>
        /// 日志写入时间
        /// </summary>
        [DisplayName("日志写入时间"), DataType(DataType.DateTime)]
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 监控项目
        /// </summary>
        [Required]
        [DisplayName("监控项目")]
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// 监视结果匹配模式
        /// </summary>
        [Required]
        [DisplayName("监视结果匹配模式")]
        public ResultTypes ResultType { get; set; }

        /// <summary>
        /// 客户端名称
        /// </summary>
        [DisplayName("客户端名称"), DataType(DataType.Text)]
        public string Client { get; set; }

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
        [DisplayName("IP地址"), DataType(DataType.Text)]
        public string IPAddress { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        [DisplayName("日志级别"), DataType(DataType.Text)]
        public string LogLevel { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [DisplayName("日志内容"), DataType(DataType.Text)]
        public string LogContent { get; set; }

    }
}
