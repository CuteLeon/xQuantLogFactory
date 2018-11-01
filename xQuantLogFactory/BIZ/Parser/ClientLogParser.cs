using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 客户端日志解析器
    /// </summary>
    public class ClientLogParser : ILogParser
    {

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        /// <remarks>2018-10-29 16:51:04,457 TRACE 安信证券 1.3.0.065 192.168.7.101 初始化准备</remarks>
        public Regex LogRegex { get; set; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<Client>.*?)\s(?<Version>.*?)\s(?<IPAddress>.*?)\s(?<LogContent>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        /// <summary>
        /// 追踪器
        /// </summary>
        public ITrace UnityTrace { get; set; }

        public IEnumerable<MonitorResult> Parse(TaskArgument argument)
        {
            throw new NotImplementedException();
        }
    }
}
