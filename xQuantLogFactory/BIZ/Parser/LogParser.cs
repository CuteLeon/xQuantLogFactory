using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Parser
{
    /// <summary>
    /// 日志解析器
    /// </summary>
    public class LogParser : ILogParser
    {
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
                    while (reader.EndOfStream)
                    {
                        //获取日志行
                        string logLine = reader.ReadLine();

                        //TODO: 使用正则匹配日志行，匹配失败时忽略
                    }
                    yield return new MonitorResult();
                    reader.Close();
                }
            }
        }

    }
}
