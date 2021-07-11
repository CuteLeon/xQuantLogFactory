using LogFactory.Model;

namespace LogFactory.BIZ.Analysiser
{
    /// <summary>
    /// 日志分析器
    /// </summary>
    public interface ILogAnalysiser
    {
        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="argument">任务参数</param>
        void Analysis(TaskArgument argument);
    }
}
