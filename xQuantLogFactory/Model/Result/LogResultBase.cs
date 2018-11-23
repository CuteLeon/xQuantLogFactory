using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 组匹配类型
    /// </summary>
    public enum GroupTypes
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
    /// 日志解析结果抽象类
    /// </summary>
    public abstract class LogResultBase
    {
        /// <summary>
        /// Gets or sets 解析结果ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("解析结果ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultID { get; set; }

        /// <summary>
        /// Gets or sets 任务参数
        /// </summary>
        [Required]
        [DisplayName("任务参数")]
        public TaskArgument TaskArgument { get; set; }

        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        [Required]
        [DisplayName("日志文件")]
        public LogFile LogFile { get; set; }

        /// <summary>
        /// Gets or sets 日志文件中的行号
        /// </summary>
        [Required]
        [DisplayName("日志文件中的行号")]
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets 日志写入时间
        /// </summary>
        [DisplayName("日志写入时间")]
        [DataType(DataType.DateTime)]
        public DateTime LogTime { get; set; }
    }
}
