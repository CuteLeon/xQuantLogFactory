using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器
    /// </summary>
    public interface ILogParser
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        void Parse(TaskArgument argument);
    }
}
