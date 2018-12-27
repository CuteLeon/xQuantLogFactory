using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BatchHost.Model.Tests
{
    [TestClass()]
    public class TaskArgumentDMTests
    {
        [TestMethod()]
        public void OnBatchesCountChangedTest()
        {
            TaskArgumentDM argument = new TaskArgumentDM();

            int count = 0;
            argument.BatchesCountChanged += new Action<int>(x => count++);

            argument.LogStartTime = DateTime.Now;
            Assert.AreEqual(1, count);
            argument.LogFinishTime = DateTime.Now;
            Assert.AreEqual(2, count);
            argument.TimeInterval = 0;
            Assert.AreEqual(3, count);
            argument.TimeIntervalUnit = FixedValue.TimeUnits.Minute;
            Assert.AreEqual(4, count);
        }

        [TestMethod()]
        public void BatchesCount()
        {
            TaskArgumentDM argument = new TaskArgumentDM();
            Assert.AreEqual(0, argument.BatchesCount);

            argument.LogStartTime = new DateTime(2000, 1, 1);
            Assert.AreEqual(0, argument.BatchesCount);

            argument.MonitorNames.Add("");
            Assert.AreEqual(1, argument.BatchesCount);

            argument.MonitorNames.Add("");
            Assert.AreEqual(2, argument.BatchesCount);

            argument.LogFinishTime = new DateTime(1000, 1, 1);
            Assert.AreEqual(0, argument.BatchesCount);

            argument.LogFinishTime = new DateTime(2000, 1, 10);
            Assert.AreEqual(2, argument.BatchesCount);

            argument.TimeInterval = 1;
            Assert.AreEqual(18, argument.BatchesCount);

            argument.TimeInterval = 5;
            Assert.AreEqual(4, argument.BatchesCount);

            argument.TimeIntervalUnit = FixedValue.TimeUnits.Hour;
            Assert.AreEqual(88, argument.BatchesCount);
        }
    }
}