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

            Assert.AreEqual(0, container.TerminalMonitorTreeRoots.Count);
            Assert.AreEqual(0, container.GetTerminalMonitorItems().Count());

            Random random = new Random();
            TerminalMonitorItem root = new TerminalMonitorItem("临时监视规则");
            TerminalMonitorItem currentMonitor = null;

            for (int index = 0; index < 100; index++)
            {
                currentMonitor = root;
                if (currentMonitor.HasChild() && random.NextDouble() > 0.3)
                    currentMonitor = currentMonitor.MonitorTreeRoots[random.Next(0, currentMonitor.MonitorTreeRoots.Count)];

                currentMonitor.MonitorTreeRoots.Add(new TerminalMonitorItem($"监视规则-{index}"));
            }
            container.TerminalMonitorTreeRoots.AddRange(root.MonitorTreeRoots);
            root = null;

            Console.WriteLine($"根节点 ({container.TerminalMonitorTreeRoots.Count}个)：{string.Join("、", container.TerminalMonitorTreeRoots.Select(monitor => monitor.Name))}");
            var monitorItems = container.GetTerminalMonitorItems().ToList();
            Console.WriteLine($"所有节点 ({monitorItems.Count}个)：{string.Join("、", monitorItems.Select(monitor => monitor.Name))}");

            Assert.IsTrue(container.TerminalMonitorTreeRoots.Count > 0 && container.TerminalMonitorTreeRoots.Count < 100);
            Assert.AreEqual(100, monitorItems.Count);
        }

        [TestMethod()]
        public void GetMonitorItemsSortTest()
        {
            MonitorContainer container = new MonitorContainer() { Name = "测试容器" };
            TerminalMonitorItem A = new TerminalMonitorItem("A");
            TerminalMonitorItem B = new TerminalMonitorItem("B");
            TerminalMonitorItem C = new TerminalMonitorItem("C");
            TerminalMonitorItem D = new TerminalMonitorItem("D");
            TerminalMonitorItem E = new TerminalMonitorItem("E");
            TerminalMonitorItem F = new TerminalMonitorItem("F");
            TerminalMonitorItem G = new TerminalMonitorItem("G");
            TerminalMonitorItem H = new TerminalMonitorItem("H");
            TerminalMonitorItem I = new TerminalMonitorItem("I");
            TerminalMonitorItem J = new TerminalMonitorItem("J");
            TerminalMonitorItem K = new TerminalMonitorItem("K");
            TerminalMonitorItem L = new TerminalMonitorItem("L");
            TerminalMonitorItem M = new TerminalMonitorItem("M");
            TerminalMonitorItem N = new TerminalMonitorItem("N");
            TerminalMonitorItem O = new TerminalMonitorItem("O");
            TerminalMonitorItem P = new TerminalMonitorItem("P");
            container.TerminalMonitorTreeRoots.AddRange(new TerminalMonitorItem[] { A, B, C });

            A.MonitorTreeRoots.AddRange(new TerminalMonitorItem[] { D, E, F });
            B.MonitorTreeRoots.AddRange(new TerminalMonitorItem[] { G, H });
            C.MonitorTreeRoots.Add(I);

            D.MonitorTreeRoots.AddRange(new TerminalMonitorItem[] { J, K });
            E.MonitorTreeRoots.Add(L);
            F.MonitorTreeRoots.Add(M);

            G.MonitorTreeRoots.Add(N);

            I.MonitorTreeRoots.AddRange(new TerminalMonitorItem[] { O, P });

            // 初始化树
            container.InitTerminalMonitorTree();
            Assert.AreSame(A, D.ParentMonitorItem);

            string result = string.Join("-", container.GetTerminalMonitorItems().Select(monitor => monitor.Name));
            Assert.AreEqual("A-D-J-K-E-L-F-M-B-G-N-H-C-I-O-P", result);
        }

        [TestMethod()]
        public void GetNextChildCANOTest()
        {
            TerminalMonitorItem root = new TerminalMonitorItem();

            Assert.AreEqual("0001", root.GetNextChildCANO());

            root.MonitorTreeRoots.Add(new TerminalMonitorItem());

            root.MonitorTreeRoots.Add(new TerminalMonitorItem() { CANO = root.GetNextChildCANO() });
            root.MonitorTreeRoots.Add(new TerminalMonitorItem() { CANO = root.GetNextChildCANO() });
            root.MonitorTreeRoots.Add(new TerminalMonitorItem() { CANO = root.GetNextChildCANO() });

            Assert.AreEqual("0001", root.MonitorTreeRoots[1].CANO);
            Assert.AreEqual("0002", root.MonitorTreeRoots[2].CANO);
            Assert.AreEqual("0003", root.MonitorTreeRoots[3].CANO);
        }
    }
}