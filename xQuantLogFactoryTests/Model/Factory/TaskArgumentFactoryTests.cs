using xQuantLogFactory.Model.Factory;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace xQuantLogFactory.Model.Factory.Tests
{
    [TestClass()]
    public class TaskArgumentFactoryTests
    {
        [TestMethod()]
        public void CreateTaskArgumentTest()
        {
            TaskArgumentFactory factory = TaskArgumentFactory.Intance;
            string[] args = new string[] {
                $@"{TaskArgumentFactory.LOG_DIR}=C:\TestDirectory1",
                $@"{TaskArgumentFactory.LOG_DIR}=C:\TestDirectory",
                $@"{TaskArgumentFactory.MONITOR_NAME}=monitor.xml",
                $@"sasdaasdlkw",
                $@"sasdaasdlkw=",
                $@"=sasdaasdlkw",
                $@"{TaskArgumentFactory.START_TIME}=sadkaw",
                $@"""{TaskArgumentFactory.FINISH_TIME}=2018-11-11 18:30:00""",
                $@"{TaskArgumentFactory.SYS_INFO}=true",
                $@"{TaskArgumentFactory.CLIENT_INFO}=false",
                $@"{TaskArgumentFactory.REPORT_MODE}=word",
            };

            TaskArgument argument = null;

            argument = factory.CreateTaskArgument(args.Take(3).ToArray());
            Assert.AreEqual(@"C:\TestDirectory", argument.LogDirectory);
            Assert.AreEqual("monitor.xml", argument.MonitorFileName);

            argument = factory.CreateTaskArgument(args);
            Assert.AreEqual(DateTime.Today, argument.LogStartTime);
            Assert.AreEqual("2018-11-11 18:30:00", argument.LogFinishTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.AreEqual(true, argument.IncludeSystemInfo);
            Assert.AreEqual(false, argument.IncludeClientInfo);
            Assert.AreEqual(ReportModes.Word, argument.ReportMode);
        }
    }
}