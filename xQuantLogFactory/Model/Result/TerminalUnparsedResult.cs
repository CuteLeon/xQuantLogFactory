using xQuantLogFactory.Model.LogFile;

namespace xQuantLogFactory.Model.Result
{
    /// <summary>
    /// 未解析结果
    /// </summary>
    public class TerminalUnparsedResult : LogResultBase<TerminalLogFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalUnparsedResult"/> class.
        /// </summary>
        public TerminalUnparsedResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalUnparsedResult"/> class.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="logFile"></param>
        /// <param name="lineNumber"></param>
        /// <param name="logContent"></param>
        public TerminalUnparsedResult(TaskArgument argument, TerminalLogFile logFile, int lineNumber, string logContent)
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
