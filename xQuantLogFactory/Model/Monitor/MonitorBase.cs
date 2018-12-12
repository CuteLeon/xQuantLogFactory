using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace xQuantLogFactory.Model.Monitor
{
    public abstract class MonitorBase : IMonitor
    {
        /// <summary>
        /// Gets or sets 项目名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets 监控项目树根节点列表
        /// </summary>
        [XmlElement("Item")]
        public virtual List<MonitorItem> MonitorTreeRoots { get; set; } = new List<MonitorItem>();

        /// <summary>
        /// Gets a value indicating whether 是否有子监控项目
        /// </summary>
        [XmlIgnore]
        public bool HasChildren => this.MonitorTreeRoots != null && this.MonitorTreeRoots.Count > 0;

        /// <summary>
        /// 获取下一个子节点的目录编号
        /// </summary>
        /// <param name="parentCANO"></param>
        /// <returns></returns>
        public virtual string GetNextChildCANO(string parentCANO = null)
        {
            /* 目录编号生成算法：
             * 才能够当前子节点目录编号取最大值，以点分隔为数组，取数组最后一个元素转换为数字，数字即为子节点中的最大编号，在此编号上增加一，即为下一个节点编号
             * 在编号数字左边补0填充，
             * 如果存在父级节点编号：继续在左边连接父级节点目录编号，并以点分隔，
             * 即为下一节点目录编号
             */
            int nextCANO = (int.TryParse(this.MonitorTreeRoots.Select(monitor => monitor.CANO ?? "0").Max()?.Split('.')?.LastOrDefault(), out int cano) ? cano : 0) + 1;
            return $"{(parentCANO == null ? string.Empty : $"{parentCANO}.")}{nextCANO.ToString("0000")}";
        }

        /// <summary>
        /// 获取所有监视规则节点（包括当前节点自身）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MonitorBase> GetMonitorBasesWithSelf()
        {
            yield return this;

            foreach (var childMonitor in this.GetMonitorItems())
            {
                yield return childMonitor;
            }
        }

        /// <summary>
        /// 查找指定名称的第一个子监视规则
        /// </summary>
        /// <param name="monitorName"></param>
        /// <returns></returns>
        /// <remarks>不要使用Linq和foreach，否则在定向分析器中对监视规则子列表做了修改后遍历会产生错误，使用索引器查找</remarks>
        public MonitorItem FindChildMonitorItem(string monitorName)
        {
            int index = 0;
            MonitorItem currentMonitor = null;

            while (index < this.MonitorTreeRoots.Count)
            {
                currentMonitor = this.MonitorTreeRoots[index];
                if (string.Equals(currentMonitor.Name, monitorName, StringComparison.OrdinalIgnoreCase))
                {
                    return currentMonitor;
                }

                index++;
            }

            return default;
        }

        /// <summary>
        /// 获取所有节点及其子节点
        /// </summary>
        /// <returns></returns>
        /// <remarks>IEnumerable<>对象即使储存为变量，每次访问依然会进入此方法，若要减少计算量，需要将此方法返回数据 .ToList()</remarks>
        public IEnumerable<MonitorItem> GetMonitorItems()
        {
            Stack<MonitorBase> monitorRoots = new Stack<MonitorBase>();
            MonitorBase currentMonitor = this;

            while (true)
            {
                if (currentMonitor.HasChildren)
                {
                    foreach (var monitor in currentMonitor.MonitorTreeRoots
                        .AsEnumerable().Reverse())
                    {
                        monitorRoots.Push(monitor);
                    }
                }

                if (monitorRoots.Count > 0)
                {
                    currentMonitor = monitorRoots.Pop();
                    yield return currentMonitor as MonitorItem;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 通用树深度优先扫描方法
        /// </summary>
        /// <param name="scanAction">扫描Action</param>
        /// <param name="stackInitPredicate">首批根节点入栈条件</param>
        /// <remarks>Action 参数分别为父级节点堆栈和当前节点，每次执行时顶级节点会出栈，下次需要扫描的子节点入栈即可，注意倒序入栈：currentMonitor.MonitorTreeRoots.AsEnumerable().Reverse().ToList()ForEach(root => stack.Push(root)</remarks>
        [Obsolete]
        public void ScanMonitor(Action<Stack<MonitorItem>, MonitorItem> scanAction, Predicate<MonitorItem> stackInitPredicate = null)
        {
            if (scanAction == null)
            {
                throw new ArgumentNullException(nameof(scanAction));
            }

            Stack<MonitorItem> monitorRoots = new Stack<MonitorItem>();
            MonitorItem currentMonitor = null;

            // 初始化栈
            ((stackInitPredicate == null) ?
                this.MonitorTreeRoots :
                this.MonitorTreeRoots.FindAll(stackInitPredicate))
                .AsEnumerable().Reverse().ToList() // 倒序入栈
                .ForEach(root => monitorRoots.Push(root));

            while (monitorRoots.Count > 0)
            {
                currentMonitor = monitorRoots.Pop();

                scanAction(monitorRoots, currentMonitor);
            }
        }
    }
}
