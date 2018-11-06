using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xQuantLogFactory.Model
{
    /// <summary>
    /// 分析结果
    /// </summary>
    [Table("AnalysisResults")]
    public class AnalysisResult
    {
        /// <summary>
        /// 分析结果ID
        /// </summary>
        [Key]
        [Required]
        [DisplayName("分析结果ID")]
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
        /// 监控项目
        /// </summary>
        [Required]
        [DisplayName("监控项目")]
        public MonitorItem MonitorItem { get; set; }

        /// <summary>
        /// 匹配开始结果
        /// </summary>
        [DisplayName("匹配开始结果")]
        public MonitorResult StartMonitorResult { get; set; }

        /// <summary>
        /// 匹配结束结果
        /// </summary>
        [DisplayName("匹配结束结果")]
        public MonitorResult FinishMonitorResult { get; set; }

        /// <summary>
        /// 结果耗时（单位：毫秒）
        /// </summary>
        [DisplayName("结果耗时（单位：毫秒）"), DataType(DataType.Duration)]
        public double ElapsedMillisecond { get; set; }

    }
}
