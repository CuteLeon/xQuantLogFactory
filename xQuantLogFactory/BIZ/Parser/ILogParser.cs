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
        //TODO: 增加一个抽象类，由客户端日志解析器、服务端日志解析器继承，抽象类中包含解析器公共方法和默认字段

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        Regex LogRegex { get; set; }

        /// <summary>
        /// 注入追踪器
        /// </summary>
        ITrace UnityTrace { get; set; }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        /// <returns></returns>
        IEnumerable<MonitorResult> Parse(TaskArgument argument);
    }
}
