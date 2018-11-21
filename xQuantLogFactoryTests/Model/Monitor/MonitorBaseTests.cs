using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class MonitorBaseTests
    {
        [TestMethod()]
        public void MonitorBaseTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "测试容器" };

            container.MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_0" });
            Assert.AreEqual(1, container.MonitorTreeRoots.Count);
            Assert.AreEqual(1, container.MonitorItems.Count);
            Assert.AreEqual(1, container.MonitorItems.Count);

            container.MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_1" });
            Assert.AreEqual(2, container.MonitorTreeRoots.Count);
            Assert.AreEqual(2, container.MonitorItems.Count);
            Assert.AreEqual(2, container.MonitorItems.Count);

            container.MonitorTreeRoots[0].MonitorTreeRoots.Add(new MonitorItem() { Name = "规则_2" });
            Assert.AreEqual(2, container.MonitorTreeRoots.Count);

            //临时解决方案：更新树根节点，请求刷新缓存列表
            container.MonitorTreeRoots.UpdateVersion();

            Assert.AreEqual(3, container.MonitorItems.Count);
        }

        [TestMethod()]
        public void PerformanceTesting()
        {
            MonitorContainer container = new MonitorContainer();
            Random random = new Random();
            MonitorBase currentMonitor = null;

            for (int index = 0; index < 100; index++)
            {
                currentMonitor = container;
                if (currentMonitor.HasChildren && random.NextDouble() > 0.3)
                    currentMonitor = currentMonitor.MonitorTreeRoots[random.Next(0, currentMonitor.MonitorTreeRoots.Count)];

                currentMonitor.MonitorTreeRoots.Add(new MonitorItem($"监视规则-{index}"));
            }

            currentMonitor.MonitorTreeRoots.UpdateVersion();
            Console.WriteLine($"根节点 ({container.MonitorTreeRoots.Count}个)：{string.Join("、", container.MonitorTreeRoots.Select(monitor => monitor.Name))}");
            Console.WriteLine($"所有节点 ({container.MonitorItems.Count}个)：{string.Join("、", container.MonitorItems.Select(monitor => monitor.Name))}");

            int exceuteCount = 15000;
            string monitorName = string.Empty;
            //测试-1：
            DateTime time_0 = DateTime.Now;
            container.MonitorTreeRoots.UpdateVersion();
            for (int index = 0; index < exceuteCount; index++)
            {
                container.MonitorItems.ForEach(monitor => monitorName = monitor.Name);
            }
            //测试-2
            DateTime time_1 = DateTime.Now;
            for (int index = 0; index < exceuteCount; index++)
            {
                container.ScanMonitor((stack, monitor) =>
                    {
                        monitorName = monitor.Name;
                    });
            }
            DateTime time_2 = DateTime.Now;

            Console.WriteLine($"方案一：耗时={(time_1 - time_0).TotalMilliseconds}");
            Console.WriteLine($"方案二：耗时={(time_2 - time_1).TotalMilliseconds}");

            //测试结果：(随机深度和结构的 100 个节点)
            /* 执行次数小于 100 时，MonitorItems [每次访问都刷新的性能] 优于 Stack ；
             * 执行次数大于 100 时，MonitorItems [每次访问都刷新的性能] 劣于 Stack ，且内存消耗更大；
             */
            /* MonitorItems [仅第一次访问时刷新] 的性能永远略优于 Stack，但存在无法感知子节点更新和空间复杂度太大的问题
             * 无法感知子节点更新解决方案：子节点更新，手动监视规则容器 UpdateVersion
             */
        }

    }
}