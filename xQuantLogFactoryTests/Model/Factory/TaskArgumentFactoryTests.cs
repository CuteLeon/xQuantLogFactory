using xQuantLogFactory.Model.Factory;
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
            /* TODO: 测试要点：
             * 缺失必选参数
             * 缺失可选参数
             * 参数重复
             * 不符合正则的传参格式
             * 缺失参数数据部分
             * 意外的参数名称
             * 意外格式的参数数据部分
             */

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
            Assert.AreEqual(argument.MonitorFileName, args[1]);
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