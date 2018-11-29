using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace xQuantLogFactory.Model.Monitor.Tests
{
    [TestClass()]
    public class MonitorBaseTests
    {

        [TestMethod()]
        public void GetMonitorItemsCountTest()
        {
            MonitorContainer container = new MonitorContainer();

            Assert.AreEqual(0, container.MonitorTreeRoots.Count);
            Assert.AreEqual(0, container.GetMonitorItems().Count());

            Random random = new Random();
            MonitorBase currentMonitor = null;

            for (int index = 0; index < 100; index++)
            {
                currentMonitor = container;
                if (currentMonitor.HasChildren && random.NextDouble() > 0.3)
                    currentMonitor = currentMonitor.MonitorTreeRoots[random.Next(0, currentMonitor.MonitorTreeRoots.Count)];

                currentMonitor.MonitorTreeRoots.Add(new MonitorItem($"监视规则-{index}"));
            }

            Console.WriteLine($"根节点 ({container.MonitorTreeRoots.Count}个)：{string.Join("、", container.MonitorTreeRoots.Select(monitor => monitor.Name))}");
            var monitorItems = container.GetMonitorItems().ToList();
            Console.WriteLine($"所有节点 ({monitorItems.Count}个)：{string.Join("、", monitorItems.Select(monitor => monitor.Name))}");

            Assert.IsTrue(container.MonitorTreeRoots.Count > 0 && container.MonitorTreeRoots.Count < 100);
            Assert.AreEqual(100, monitorItems.Count);
        }

        [TestMethod()]
        public void GetMonitorItemsSortTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "测试容器" };
            MonitorItem A = new MonitorItem("A");
            MonitorItem B = new MonitorItem("B");
            MonitorItem C = new MonitorItem("C");
            MonitorItem D = new MonitorItem("D");
            MonitorItem E = new MonitorItem("E");
            MonitorItem F = new MonitorItem("F");
            MonitorItem G = new MonitorItem("G");
            MonitorItem H = new MonitorItem("H");
            MonitorItem I = new MonitorItem("I");
            MonitorItem J = new MonitorItem("J");
            MonitorItem K = new MonitorItem("K");
            MonitorItem L = new MonitorItem("L");
            MonitorItem M = new MonitorItem("M");
            MonitorItem N = new MonitorItem("N");
            MonitorItem O = new MonitorItem("O");
            MonitorItem P = new MonitorItem("P");
            container.MonitorTreeRoots.AddRange(new MonitorItem[] { A, B, C });

            A.MonitorTreeRoots.AddRange(new MonitorItem[] { D, E, F });
            B.MonitorTreeRoots.AddRange(new MonitorItem[] { G, H });
            C.MonitorTreeRoots.Add(I);

            D.MonitorTreeRoots.AddRange(new MonitorItem[] { J, K });
            E.MonitorTreeRoots.Add(L);
            F.MonitorTreeRoots.Add(M);

            G.MonitorTreeRoots.Add(N);

            I.MonitorTreeRoots.AddRange(new MonitorItem[] { O, P });

            // 初始化树
            container.InitMonitorTree();
            Assert.AreSame(container, A.ParentMonitorContainer);
            Assert.AreSame(A, D.ParentMonitorItem);

            string result = string.Join("-", container.GetMonitorItems().Select(monitor => monitor.Name));
            Assert.AreEqual("A-D-J-K-E-L-F-M-B-G-N-H-C-I-O-P", result);
        }

    }
}