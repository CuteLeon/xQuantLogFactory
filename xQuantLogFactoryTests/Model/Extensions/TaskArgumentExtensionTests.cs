using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Model.Extensions.Tests
{
    [TestClass()]
    public class TaskArgumentExtensionTests
    {
        [TestMethod()]
        public void CheckTaskTime()
        {
            TaskArgument argument = new TaskArgument();

            // 检查文件时间
            argument.LogStartTime = null;
            argument.LogFinishTime = null;
            Assert.IsTrue(argument.CheckLogFileTime(DateTime.MinValue, DateTime.MaxValue));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = null;
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(500), new DateTime(800)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(500), new DateTime(1500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(2000), new DateTime(3000)));

            argument.LogStartTime = null;
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1000), new DateTime(1500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1500), new DateTime(2500)));
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(2500), new DateTime(3000)));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(500), new DateTime(800)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(500), new DateTime(1500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(500), new DateTime(2500)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1500), new DateTime(1800)));
            Assert.IsTrue(argument.CheckLogFileTime(new DateTime(1500), new DateTime(2500)));
            Assert.IsFalse(argument.CheckLogFileTime(new DateTime(2500), new DateTime(3000)));

            // 检查日志时间
            argument.LogStartTime = null;
            argument.LogFinishTime = null;
            Assert.IsTrue(argument.CheckLogTime(DateTime.MinValue));
            Assert.IsTrue(argument.CheckLogTime(DateTime.MaxValue));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = null;
            Assert.IsFalse(argument.CheckLogTime(new DateTime(500)));
            Assert.IsTrue(argument.CheckLogTime(new DateTime(1500)));

            argument.LogStartTime = null;
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsTrue(argument.CheckLogTime(new DateTime(1000)));
            Assert.IsFalse(argument.CheckLogTime(new DateTime(2500)));

            argument.LogStartTime = new DateTime(1000);
            argument.LogFinishTime = new DateTime(2000);
            Assert.IsFalse(argument.CheckLogTime(new DateTime(500)));
            Assert.IsTrue(argument.CheckLogTime(new DateTime(1500)));
            Assert.IsFalse(argument.CheckLogTime(new DateTime(2500)));
        }
    }
}