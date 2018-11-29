using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Model.Extensions;
using xQuantLogFactory.Model.Result;
using xQuantLogFactory.Utils.Trace;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 中间件日志解析器
    /// </summary>
    public class MiddlewareLogParser : LogParserBase
    {
        public MiddlewareLogParser()
        {
        }

        public MiddlewareLogParser(ITracer tracer)
            : base(tracer)
        {
        }

        public override Regex GeneralLogRegex { get; } = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Client>\d{1,3}(\.\d{1,3}){3})\s(?<UserCode>.*?)\s(?<StartTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}.\d{3})\s(?<Elapsed>\d*?)\s(?<RequestURI>\/.*?)\s(?<MethodName>.*?)\s(?<StreamLength>\d*?)\s(?<Message>.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument">任务参数</param>
        public override void Parse(TaskArgument argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            // 遍历文件
            argument.LogFiles.Where(file => file.LogFileType == LogFileTypes.Middleware).AsParallel().ForAll(logFile =>
            {
                this.Tracer?.WriteLine($"开始解析日志文件：(ID: {logFile.RelativePath}, Type: {logFile.LogFileType}) {logFile.RelativePath}");

                FileStream fileStream = null;
                StreamReader streamRreader = null;
                try
                {
                    fileStream = new FileStream(logFile.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    streamRreader = new StreamReader(fileStream, Encoding.Default);

                    // 临时变量放于循环外，防止内存爆炸
                    Match match = null;
                    string logLine = string.Empty;
                    int lineNumber = 0;
                    DateTime logTime = DateTime.MinValue;

                    while (!streamRreader.EndOfStream)
                    {
                        lineNumber++;

                        // 获取日志行
                        logLine = streamRreader.ReadLine();
                        match = this.GeneralLogRegex.Match(logLine);
                        if (match.Success)
                        {
                            // 跳过日志时间在任务时间范围外的日志行
                            if (!match.Groups["LogTime"].Success ||
                                !DateTime.TryParse(match.Groups["LogTime"].Value, out logTime))
                            {
                                continue;
                            }

                            if (!argument.CheckLogStartTime(logTime) ||
                                !argument.CheckLogFinishTime(logTime))
                            {
                                continue;
                            }

                            MiddlewareResult result = new MiddlewareResult(logTime, lineNumber);

                            // 反向关联日志解析结果
                            lock (argument)
                            {
                                result.TaskArgument = argument;
                                result.LogFile = logFile;

                                argument.MiddlewareResults.Add(result);
                                logFile.MiddlewareResults.Add(result);
                            }

                            if (match.Groups["Client"].Success)
                            {
                                result.Client = match.Groups["Client"].Value;
                            }

                            if (match.Groups["UserCode"].Success)
                            {
                                result.UserCode = match.Groups["UserCode"].Value;
                            }

                            if (match.Groups["StartTime"].Success)
                            {
                                result.StartTime = match.Groups["StartTime"].Value;
                            }

                            if (match.Groups["Elapsed"].Success && int.TryParse(match.Groups["Elapsed"].Value, out int elapsed))
                            {
                                result.Elapsed = elapsed;
                            }

                            if (match.Groups["StreamLength"].Success && int.TryParse(match.Groups["StreamLength"].Value, out int streamLength))
                            {
                                result.StreamLength = streamLength;
                            }

                            if (match.Groups["Message"].Success)
                            {
                                result.Message = match.Groups["Message"].Value;
                            }

                            if (match.Groups["RequestURI"].Success)
                            {
                                result.RequestURI = match.Groups["RequestURI"].Value;
                            }

                            if (match.Groups["MethodName"].Success)
                            {
                                result.MethodName = match.Groups["MethodName"].Value;
                            }
                        }
                        else
                        {
                            // 未识别的日志行
                        }
                    }

                    this.Tracer?.WriteLine($"当前日志文件(ID: {logFile.RelativePath})解析完成：{logFile.MiddlewareResults.Count} 个结果\n————————");
                }
                catch (Exception ex)
                {
                    this.Tracer?.WriteLine($"解析中间件日志文件(ID: {logFile.RelativePath}) {logFile.RelativePath} 失败：{ex.Message}\n————————");
                }
                finally
                {
                    streamRreader?.Close();
                    streamRreader?.Dispose();

                    fileStream?.Close();
                    fileStream?.Dispose();
                }
            });
        }
    }
}
