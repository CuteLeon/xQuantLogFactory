using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model
{
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
        public string FileID { get; set; }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        [Required]
        [DisplayName("日志文件路径"), DataType(DataType.Text)]
        public string FilePath { get; set; }

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

    }
}
