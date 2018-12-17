using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Model.Extensions.Tests
{
    [TestClass()]
    public class TaskArgumentExtensionTests
    {
        [TestMethod()]
        public void CanCheckLogStartTimeTest()
        {
            TaskArgument argument = new TaskArgument();

            Assert.IsFalse(argument.CanCheckLogStartTime());
            Assert.IsFalse(argument.CanCheckLogFinishTime());

            argument.LogStartTime = DateTime.Today.AddDays(-2);
            Assert.IsTrue(argument.CanCheckLogStartTime());
            Assert.IsTrue(argument.CheckLogStartTime(DateTime.Today));
            Assert.IsFalse(argument.CheckLogStartTime(DateTime.Today.AddDays(-7)));

            argument.LogFinishTime = DateTime.Today.AddDays(-1);
            Assert.IsTrue(argument.CanCheckLogFinishTime());
            Assert.IsTrue(argument.CanCheckLogFinishTime());
            Assert.IsTrue(argument.CheckLogFinishTime(DateTime.Today.AddDays(-3)));
            Assert.IsFalse(argument.CheckLogFinishTime(DateTime.Today));
        }
    }
}