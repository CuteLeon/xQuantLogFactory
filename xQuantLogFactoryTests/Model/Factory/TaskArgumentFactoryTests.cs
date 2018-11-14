﻿using xQuantLogFactory.Model.Factory;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Model.Factory.Tests
{
    [TestClass()]
    public class TaskArgumentFactoryTests
    {
        [TestMethod()]
        public void CreateTaskArgumentTest()
        {
            TaskArgumentFactory factory = new TaskArgumentFactory();
            string[] args = new string[] {
                @"logdir=C:\TestDirectory",
                "监视项目_0,监视项目_1,监视项目_2",
                "2018-10-30 15:25:01",
                "2018-10-30 18:10:05",
                "false",
                "TRUE",
                "Word",
            };

            TaskArgument argument = null;

            argument = factory.CreateTaskArgument(args.Take(3).ToArray());
            Assert.AreEqual(argument.LogDirectory, args[0]);
            Assert.AreEqual(argument.MonitorItemNames.Count, args[1].Split(',').Length);
            Assert.AreEqual(argument.LogStartTime?.ToString("yyyy-MM-dd HH:mm:ss"), args[2]);

            argument = factory.CreateTaskArgument(args.Take(4).ToArray());
            Assert.AreEqual(argument.LogFinishTime?.ToString("yyyy-MM-dd HH:mm:ss"), args[3]);

            argument = factory.CreateTaskArgument(args.Take(5).ToArray());
            Assert.AreEqual(argument.IncludeSystemInfo.ToString().ToUpper(), args[4].ToUpper());

            argument = factory.CreateTaskArgument(args.Take(6).ToArray());
            Assert.AreEqual(argument.IncludeClientInfo.ToString().ToUpper(), args[5].ToUpper());

            argument = factory.CreateTaskArgument(args.Take(7).ToArray());
            Assert.AreEqual(argument.ReportMode.ToString().ToUpper(), args[6].ToUpper());
        }
    }
}