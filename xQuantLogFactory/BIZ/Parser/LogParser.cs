using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using xQuantLogFactory.Model;
using xQuantLogFactory.Utils;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器
    /// </summary>
    public class LogParser : ILogParser
    {
        //TODO: 客户端分析日志内IP地址

        /// <summary>
        /// 日志正则表达式
        /// </summary>
        /// <remarks>2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券</remarks>
        public Regex LogRegex = new Regex(
            @"^(?<LogTime>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2}),(?<Millisecond>\d{0,3})\s(?<LogLevel>(TRACE|DEBUG|INFO|WARN))\s(?<Client>.*?)\s(?<Version>.*?)\s(?<LogContent>.*)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 追踪器
        /// </summary>
        public ITrace UnityTrace { get; set; }

        /// <summary>
        /// 日志解析
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public IEnumerable<MonitorResult> Parse(TaskArgument argument)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            //遍历文件
            foreach (LogFile logFile in argument.LogFiles)
            {
                this.UnityTrace?.WriteLine($"开始解析日志文件：(ID: {logFile.FileID}) {logFile.FilePath}");

                using (StreamReader reader = new StreamReader(logFile.FilePath, Encoding.Default))
                {
                    int lineNumber = 0;
                    while (!reader.EndOfStream)
                    {
                        lineNumber++;
                        //获取日志行
                        string logLine = reader.ReadLine();
                        Match match = this.LogRegex.Match(logLine);
                        if (match.Success)
                        {
                            //跳过未匹配到内容的日志行
                            if (!match.Groups["LogContent"].Success) continue;

                            string logContent = match.Groups["LogContent"].Value;
                            //匹配所有监视规则
                            foreach (MonitorItem monitor in argument.MonitorItems)
                            {
                                if (monitor.StartRegex != null && monitor.StartRegex.IsMatch(logContent))
                                {
                                    //TODO: 开始匹配
                                }
                                else if (monitor.FinishRegex != null && monitor.FinishRegex.IsMatch(logContent))
                                {
                                    //TODO: 结束匹配
                                }
                                else if (monitor.ItemRegex != null && monitor.ItemRegex.IsMatch(logContent))
                                {
                                    //TODO: 条目匹配
                                }
                                else
                                {
                                    //未匹配到任何监视规则则尝试下一条规则
                                    continue;
                                }
                                MonitorResult result = new MonitorResult()
                                {
                                    LineNumber = lineNumber,
                                    LogFile = logFile,
                                    LogContent = logContent,
                                };

                                if (match.Groups["LogLevel"].Success)
                                    result.LogLevel = match.Groups["LogLevel"].Value;

                                //不能从此格式直接转换为日期时间类型，需要分开处理
                                if (match.Groups["LogTime"].Success &&
                                    DateTime.TryParse(match.Groups["LogTime"].Value, out DateTime logTime))
                                    result.LogTime = logTime;

                                if (match.Groups["Millisecond"].Success &&
                                    double.TryParse(match.Groups["Millisecond"].Value, out double millisecond))
                                    result.LogTime = result.LogTime.AddMilliseconds(millisecond);

                                if (match.Groups["Version"].Success)
                                    result.Version = match.Groups["Version"].Value;

                                if (match.Groups["Client"].Success)
                                    result.Client = match.Groups["Client"].Value;
                                result.MonitorItem = null;

                                yield return result;
                            }
                        }
                        else
                        {
                            //未识别的日志行
                        }
                    }

                    reader.Close();
                    this.UnityTrace?.WriteLine("当前日志文件解析完成");
                }
            }
        }

    }
}
