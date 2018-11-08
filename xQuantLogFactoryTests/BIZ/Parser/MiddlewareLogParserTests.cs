using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using xQuantLogFactory.Model;

namespace xQuantLogFactory.BIZ.Parser.Tests
{
    [TestClass()]
    public class MiddlewareLogParserTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            TaskArgument argument = new TaskArgument()
            {
                TaskID = Guid.NewGuid().ToString("N"),
                LogStartTime = new DateTime(2018, 10, 30, 12, 04, 0, 0),
            };
            argument.LogFinishTime = argument.LogStartTime?.AddSeconds(5);
            argument.LogFiles.Add(new LogFile()
            {
                LogFileType = LogFileTypes.Middleware,
                FilePath = @"D:\xQuant\TT\日志分析工具\Log.Zip\log_2018-10-31_11-01\log\performanceLog20181030.txt",
            });

            Assert.AreEqual("2018-10-30 12:04:00.000", argument.LogStartTime?.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.AreEqual("2018-10-30 12:04:05.000", argument.LogFinishTime?.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Assert.AreEqual(1, argument.LogFiles.Count);
            Assert.AreEqual("performanceLog20181030.txt", Path.GetFileName(argument.LogFiles.First().FilePath));

            ILogParser parser = new MiddlewareLogParser();
            parser.Parse(argument);
            Assert.IsTrue(argument.MiddlewareResults.Count > 0);
            Console.WriteLine(argument.MiddlewareResults.Count);
        }
    }
}