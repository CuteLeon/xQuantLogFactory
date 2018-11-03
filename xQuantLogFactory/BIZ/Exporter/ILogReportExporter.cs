using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Exporter
{
    //TODO: 实现其他格式报告导出器

    /// <summary>
    /// 日志报告导出器接口
    /// </summary>
    public interface ILogReportExporter
    {
        /// <summary>
        /// 导出日志报告
        /// </summary>
        /// <param name="reportPath">报告文件路径</param>
        /// <param name="argument">任务参数</param>
        void ExportReport(string reportPath, TaskArgument argument);
    }
}
