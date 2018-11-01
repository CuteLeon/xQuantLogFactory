using System.Collections.Generic;
using System.Text.RegularExpressions;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

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
        /// <returns></returns>
        IEnumerable<MonitorResult> Parse(TaskArgument argument);
    }
}
