using Microsoft.VisualStudio.TestTools.UnitTesting;
using xQuantLogFactory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xQuantLogFactory.Model.Tests
{
    [TestClass()]
    public class TaskArgumentTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            string[] args = new string[] {
                @"C:\TestDirectory",
                "监视项目_0,监视项目_1,监视项目_2",
                "2018-10-30 15:25:01",
                "2018-10-30 18:10:05",
                "false",
                "TRUE",
                "Word",
            };

            TaskArgument argument = null;

            argument = TaskArgument.Parse(args.Take(3).ToArray());
            Assert.AreEqual(argument.BaseDirectory, args[0]);
            Assert.AreEqual(argument.ItemNames.Count, args[1].Split(',').Length);
            Assert.AreEqual(argument.LogStartTime.ToString("yyyy-MM-dd HH:mm:ss"), args[2]);

            argument = TaskArgument.Parse(args.Take(4).ToArray());
            Assert.AreEqual(argument.LogFinishTime.ToString("yyyy-MM-dd HH:mm:ss"), args[3]);

            argument = TaskArgument.Parse(args.Take(5).ToArray());
            Assert.AreEqual(argument.IncludeSystemInfo.ToString().ToUpper(), args[4].ToUpper());

            argument = TaskArgument.Parse(args.Take(6).ToArray());
            Assert.AreEqual(argument.IncludeClientInfo.ToString().ToUpper(), args[5].ToUpper());

            argument = TaskArgument.Parse(args.Take(7).ToArray());
            Assert.AreEqual(argument.ReportMode.ToString().ToUpper(), args[6].ToUpper());
        }
    }
}