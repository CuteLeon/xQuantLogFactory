using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 中间件日志解析器
    /// </summary>
    public class MiddlewareLogParser : LogParserBase
    {

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        public override Regex LogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Client>.*?)\s(?<UserCode>.*?)\s(?<StartTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Elapsed>.*?)\s(?<RequestURI>.*?)\s(?<MethodName>.*?)\s(?<StreamLength>.*?)\s(?<Message>.+)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public MiddlewareLogParser() { }

        public MiddlewareLogParser(ITrace trace) : base(trace) { }

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void Parse(TaskArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //遍历文件
            argument.LogFiles.Where(file => file.LogFileType == LogFileTypes.Middleware).AsParallel().ForAll(logFile =>
            {
                this.Trace?.WriteLine($"开始解析日志文件：(ID: {logFile.FileID}, Type: {logFile.LogFileType}) {logFile.FilePath}");

                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(logFile.FilePath, Encoding.Default);

                    int lineNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        lineNumber++;
                        //获取日志行
                        string logLine = reader.ReadLine();
                        Match match = this.LogRegex.Match(logLine);
                        if (match.Success)
                        {
                            DateTime logTime = DateTime.MinValue;
                            //跳过日志时间在任务时间范围外的日志行
                            if (!match.Groups["LogTime"].Success ||
                                !DateTime.TryParse(match.Groups["LogTime"].Value, out logTime) ||
                                logTime < argument.LogStartTime ||
                                logTime > argument.LogFinishTime
                                )
                                continue;

                            MiddlewareResult result = new MiddlewareResult()
                            {
                                LogTime = logTime,
                                LineNumber = lineNumber,
                            };

                            //反向关联日志解析结果
                            lock (argument)  //lock 任务而非 LockSeed 为了多任务并行考虑
                            {
                                result.TaskArgument = argument;
                                result.LogFile = logFile;

                                argument.MiddlewareResults.Add(result);
                                logFile.MiddlewareResults.Add(result);
                            }

                            if (match.Groups["Client"].Success)
                                result.Client = match.Groups["Client"].Value;

                            if (match.Groups["UserCode"].Success)
                                result.UserCode = match.Groups["UserCode"].Value;

                            if (match.Groups["StartTime"].Success)
                                result.StartTime = match.Groups["StartTime"].Value;

                            if (match.Groups["Elapsed"].Success && int.TryParse(match.Groups["Elapsed"].Value, out int elapsed))
                                result.Elapsed = elapsed;

                            if (match.Groups["StreamLenth"].Success && int.TryParse(match.Groups["StreamLenth"].Value, out int streamLength))
                                result.StreamLenth = streamLength;

                            if (match.Groups["Message"].Success)
                                result.Message = match.Groups["Message"].Value;

                            if (match.Groups["RequestURI"].Success)
                                result.RequestURI = match.Groups["RequestURI"].Value;

                            if (match.Groups["MethodName"].Success)
                                result.StartTime = match.Groups["MethodName"].Value;
                        }
                        else
                        {
                            //未识别的日志行
                        }
                    }

                    this.Trace?.WriteLine($"当前日志文件(ID: {logFile.FileID})解析完成：{logFile.MiddlewareResults.Count} 个结果\n————————");
                }
                catch (Exception ex)
                {
                    this.Trace?.WriteLine($"解析中间件日志文件(ID: {logFile.FileID}) {logFile.FilePath} 失败：{ex.Message}\n————————");
                }
                finally
                {
                    reader?.Close();
                    reader?.Dispose();
                }
            });
        }

    }
}
