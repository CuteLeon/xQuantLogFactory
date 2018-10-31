using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器
    /// </summary>
    public class LogParser : ILogParser
    {
        /// <summary>
        /// 日志正则表达式
        /// </summary>
        /// <remarks>2018-10-30 09:25:30,111 DEBUG 东方证券 1.3.0.064补丁1 开始排券</remarks>
        public Regex LogRegex = new Regex(
            @"(?<Time>\d{4}-\d{1,2}-\d{1,2}\s\d{2}:\d{2}:\d{2},\d{3})\s(?<Type>(TRACE|DEBUG|INFO|WARN))",//*$
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

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
                using (StreamReader reader = new StreamReader(logFile.FilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        //获取日志行
                        string logLine = reader.ReadLine();
                        Match match = this.LogRegex.Match(logLine);

                        if (match.Success)
                        {
                            //TODO: 使用正则匹配日志行
                            Console.WriteLine(match.Groups["Time"]);
                            Console.WriteLine(match.Groups["Type"]);
                        }
                        else
                        {
                            //未识别的日志行
                        }
                    }
                    yield return new MonitorResult();
                    reader.Close();
                }
            }
        }

    }
}
