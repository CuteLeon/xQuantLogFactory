using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 未解析结果
    /// </summary>
    public class UnparsedResult : LogResultBase
    {
        public UnparsedResult()
        {
        }

        public UnparsedResult(TaskArgument argument, LogFile logFile, int lineNumber, string logContent)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
            this.LineNumber = lineNumber;
            this.LogContent = logContent;
        }

        /// <summary>
        /// Gets or sets 日志内容
        /// </summary>
        public string LogContent { get; set; }
    }
}
