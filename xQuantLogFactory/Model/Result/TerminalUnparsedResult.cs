using xQuantLogFactory.Model.LogFile;
using xQuantLogFactory.Model.Monitor;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 未解析结果
    /// </summary>
    public class TerminalUnparsedResult : LogResultBase
    {
        public TerminalUnparsedResult()
        {
        }

        public TerminalUnparsedResult(TaskArgument argument, TerminalLogFile logFile, int lineNumber, string logContent)
        {
            this.TaskArgument = argument;
            this.LogFile = logFile;
            this.LineNumber = lineNumber;
            this.LogContent = logContent;
        }

        /// <summary>
        /// Gets or sets 日志文件
        /// </summary>
        public TerminalLogFile LogFile { get; set; }

        /// <summary>
        /// Gets or sets 日志内容
        /// </summary>
        public string LogContent { get; set; }
    }
}
